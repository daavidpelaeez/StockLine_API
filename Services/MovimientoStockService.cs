using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StockLine_API.DTOs;
using StockLine_API.Models;

namespace StockLine_API.Services
{
    public class MovimientoStockService
    {
        private readonly StockLineContext _context;

        public MovimientoStockService(StockLineContext context) { _context = context; }

    
        public MovimientoStock Create(MovimientoStock m)
        {
            m.Fecha = DateTime.Now;
            _context.MovimientosStock.Add(m);
            _context.SaveChanges();
            return m;
        }


        public List<MovimientoStockHistoryItemDTO> GetFiltered(
            int? productId,
            int? usuarioId,
            string? tipo,
            DateTime? from,
            DateTime? to,
            string sortBy = "Fecha",
            string sortDir = "desc",
            int page = 1,
            int pageSize = 25)
        {
            var q = _context.MovimientosStock.AsNoTracking()
                .Include(m => m.Producto)
                .Include(m => m.Usuario)
                .AsQueryable();

            if (productId.HasValue) q = q.Where(m => m.ProductoID == productId.Value);
            if (usuarioId.HasValue) q = q.Where(m => m.UsuarioID == usuarioId.Value);
            if (!string.IsNullOrWhiteSpace(tipo))
                q = q.Where(m => m.TipoMovimiento != null && m.TipoMovimiento.ToLower() == tipo.ToLower());
            if (from.HasValue) q = q.Where(m => m.Fecha >= from.Value.Date);
            if (to.HasValue) q = q.Where(m => m.Fecha <= to.Value.Date.AddDays(1).AddTicks(-1));


            bool asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
            q = sortBy switch
            {
                "ProductoNombre" => asc ? q.OrderBy(m => m.Producto.Nombre) : q.OrderByDescending(m => m.Producto.Nombre),
                "Cantidad" => asc ? q.OrderBy(m => m.Cantidad) : q.OrderByDescending(m => m.Cantidad),
                _ => asc ? q.OrderBy(m => m.Fecha) : q.OrderByDescending(m => m.Fecha),
            };

            // Paginación real en la consulta
            var itemsQuery = q.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Solo los campos necesarios en el DTO
            var itemsDto = itemsQuery.Select(m => new MovimientoStockHistoryItemDTO
            {
                MovimientoID = m.MovimientoID,
                Fecha = m.Fecha,
                ProductoID = m.ProductoID,
                ProductoNombre = m.Producto?.Nombre ?? string.Empty,
                Cantidad = m.Cantidad,
                TipoMovimiento = m.TipoMovimiento,
                UsuarioID = m.UsuarioID,
                UsuarioNombre = m.Usuario != null ? $"{m.Usuario.Nombre} {m.Usuario.Apellidos}" : string.Empty,
                StockAfter = null // Si necesitas este campo, calcula solo si es estrictamente necesario
            }).ToList();

            return itemsDto;
        }

        public MovimientoStockHistoryItemDTO? GetById(int id)
        {
            var m = _context.MovimientosStock
                .Include(x => x.Producto)
                .Include(x => x.Usuario)
                .FirstOrDefault(x => x.MovimientoID == id);

            if (m == null) return null;

            int? stockAfter = null;
            if (m.ProductoID.HasValue)
            {
                var currentStock = _context.Productos
                    .Where(p => p.ProductoID == m.ProductoID)
                    .Select(p => p.Stock)
                    .FirstOrDefault();

                var laterDeltas = _context.MovimientosStock
                    .Where(x => x.ProductoID == m.ProductoID && x.MovimientoID > m.MovimientoID)
                    .AsEnumerable()
                    .Sum(x => string.Equals(x.TipoMovimiento, "Entrada", StringComparison.OrdinalIgnoreCase) ? x.Cantidad : -x.Cantidad);

                stockAfter = currentStock - laterDeltas;
            }

            List<MovimientoProductoDetalleDTO>? productos = null;
            if (!string.IsNullOrEmpty(m.Observaciones) && m.Observaciones.StartsWith("Envio "))
            {
                var envioIdStr = m.Observaciones.Split(' ')[1];
                if (int.TryParse(envioIdStr, out int envioId))
                {
                    var envio = _context.Envios
                        .Include(e => e.Detalles)
                        .ThenInclude(d => d.Producto)
                        .FirstOrDefault(e => e.EnvioID == envioId);
                    if (envio != null)
                    {
                        productos = envio.Detalles.Select(d => new MovimientoProductoDetalleDTO
                        {
                            ProductoID = d.ProductoID, 
                            ProductoNombre = d.Producto?.Nombre ?? string.Empty,
                            Cantidad = d.Cantidad
                        }).ToList();
                    }
                }
            }

            return new MovimientoStockHistoryItemDTO
            {
                MovimientoID = m.MovimientoID,
                Fecha = m.Fecha,
                ProductoID = m.ProductoID, 
                ProductoNombre = m.Producto?.Nombre ?? string.Empty,
                Cantidad = m.Cantidad,
                TipoMovimiento = m.TipoMovimiento,
                UsuarioID = m.UsuarioID,
                UsuarioNombre = m.Usuario != null ? $"{m.Usuario.Nombre} {m.Usuario.Apellidos}" : string.Empty,
                Observaciones = m.Observaciones,
                StockAfter = stockAfter,
                Productos = productos
            };
        }

        public (MovimientoStock movimiento, int stockAfter) Create(MovimientoStockDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Cantidad <= 0) throw new ArgumentException("Cantidad must be greater than 0", nameof(dto.Cantidad));
            if (string.IsNullOrWhiteSpace(dto.TipoMovimiento)) throw new ArgumentException("TipoMovimiento is required", nameof(dto.TipoMovimiento));

            using var tx = _context.Database.BeginTransaction();

            var producto = _context.Productos.FirstOrDefault(p => p.ProductoID == dto.ProductoID);
            if (producto == null) throw new KeyNotFoundException("Producto no encontrado");

            var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == dto.UsuarioID);
            if (usuario == null) throw new KeyNotFoundException("Usuario no encontrado");

            var tipo = dto.TipoMovimiento.Trim();
            int newStock = producto.Stock;

            // Verificación robusta de duplicados para envíos
            bool yaExiste = false;
            if (string.Equals(tipo, "Salida", StringComparison.OrdinalIgnoreCase) && dto.Observaciones != null && dto.Observaciones.StartsWith("Envio "))
            {
                yaExiste = _context.MovimientosStock.Any(m =>
                    m.ProductoID == dto.ProductoID &&
                    m.TipoMovimiento == "Salida" &&
                    m.Observaciones == dto.Observaciones);
            }
            else if (string.Equals(tipo, "Entrada", StringComparison.OrdinalIgnoreCase))
            {
                yaExiste = _context.MovimientosStock.Any(m =>
                    m.ProductoID == dto.ProductoID &&
                    m.TipoMovimiento == "Entrada" &&
                    m.Observaciones == dto.Observaciones);
            }

            if (yaExiste)
            {
                var movimientoExistente = _context.MovimientosStock.First(m =>
                    m.ProductoID == dto.ProductoID &&
                    m.TipoMovimiento == tipo &&
                    m.Observaciones == dto.Observaciones);
                tx.Commit();
                movimientoExistente.Producto = producto;
                movimientoExistente.Usuario = usuario;
                return (movimientoExistente, producto.Stock);
            }

            // Solo modificar el stock si el movimiento NO existe
            if (string.Equals(tipo, "Salida", StringComparison.OrdinalIgnoreCase))
            {
                if (producto.Stock < dto.Cantidad)
                {
                    throw new InvalidOperationException($"No hay suficiente stock para realizar la salida. Stock actual: {producto.Stock}");
                }
                producto.Stock -= dto.Cantidad;
                newStock = producto.Stock;
            }
            else if (string.Equals(tipo, "Entrada", StringComparison.OrdinalIgnoreCase))
            {
                producto.Stock += dto.Cantidad;
                newStock = producto.Stock;
            }

            _context.SaveChanges();

            var movimiento = new MovimientoStock
            {
                ProductoID = dto.ProductoID,
                Cantidad = dto.Cantidad,
                TipoMovimiento = tipo,
                UsuarioID = dto.UsuarioID,
                Observaciones = dto.Observaciones,
                Fecha = DateTime.Now
            };

            _context.MovimientosStock.Add(movimiento);
            _context.SaveChanges();

            tx.Commit();

            movimiento.Producto = producto;
            movimiento.Usuario = usuario;

            return (movimiento, newStock);
}
        public object GetSummary(int? productId, DateTime? from, DateTime? to)
{
    var q = _context.MovimientosStock.AsQueryable();
    if (productId.HasValue) q = q.Where(m => m.ProductoID == productId.Value);
    if (from.HasValue) q = q.Where(m => m.Fecha >= from.Value.Date);
    if (to.HasValue) q = q.Where(m => m.Fecha <= to.Value.Date.AddDays(1).AddTicks(-1));

    var entradas = q.Where(m => m.TipoMovimiento != null && m.TipoMovimiento.ToLower() == "entrada").Sum(m => (int?)m.Cantidad) ?? 0;
    var salidas = q.Where(m => m.TipoMovimiento != null && m.TipoMovimiento.ToLower() == "salida").Sum(m => (int?)m.Cantidad) ?? 0;
    return new { TotalEntradas = entradas, TotalSalidas = salidas, Neto = entradas - salidas };
}
    }
}
