using StockLine_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using StockLine_API.DTOs;

namespace StockLine_API.Services
{
    public class ProductoService
    {
        private readonly StockLineContext _context;

        public ProductoService(StockLineContext context)
        {
            _context = context;
        }

        public List<Producto> GetAll() => _context.Productos
            .Include(p => p.Categoria)
            .ToList();

        public Producto Get(int id) => _context.Productos
            .Include(p => p.Categoria)
            .FirstOrDefault(p => p.ProductoID == id);

        public Producto Create(Producto p)
        {
            _context.Productos.Add(p);
            _context.SaveChanges();
            return p;
        }

        public void Update(Producto p)
        {
            var e = _context.Productos.Find(p.ProductoID);
            if (e != null)
            {
                e.Nombre = p.Nombre;
                e.Descripcion = p.Descripcion;
                e.Stock = p.Stock;
                e.Foto = p.Foto;
                e.CategoriaID = p.CategoriaID;
                _context.SaveChanges();
            }
        }

        public void UpdateStockManual(int productoId, int nuevoStock, int usuarioID)
        {
            var e = _context.Productos.Find(productoId);
            if (e != null)
            {
                int stockAnterior = e.Stock;
                int diferencia = nuevoStock - stockAnterior;
                e.Stock = nuevoStock;
                _context.SaveChanges();

                if (diferencia != 0)
                {
                    var movimiento = new MovimientoStock
                    {
                        ProductoID = productoId,
                        Cantidad = Math.Abs(diferencia),
                        TipoMovimiento = diferencia > 0 ? "Entrada" : "Salida",
                        UsuarioID = usuarioID,
                        Observaciones = $"Modificación manual de stock ({DateTime.Now:yyyy-MM-dd HH:mm:ss})",
                        Fecha = DateTime.Now
                    };
                    _context.MovimientosStock.Add(movimiento);
                    _context.SaveChanges();
                }
            }
        }

        public void Delete(int id)
        {
            var e = _context.Productos.Find(id);
            if (e != null)
            {
                _context.Productos.Remove(e);
                _context.SaveChanges();
            }
        }

        
        public void SetPhoto(int productoId, IFormFile file)
        {
            var p = _context.Productos.Find(productoId);
            if (p == null) return;

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"producto_{productoId}_{DateTime.Now.Ticks}{extension}";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "productos");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            p.Foto = $"/images/productos/{fileName}";
            _context.SaveChanges();
        }

        public void RemovePhoto(int productoId)
        {
            var p = _context.Productos.Find(productoId);
            if (p == null) return;

            if (!string.IsNullOrEmpty(p.Foto))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", p.Foto.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            p.Foto = null;
            _context.SaveChanges();
        }

        public List<ProductoDTO> GetAllDTO()
        {
            return _context.Productos.Include(p => p.Categoria).Select(p => new ProductoDTO
            {
                ProductoID = p.ProductoID,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Stock = p.Stock,
                CategoriaID = p.CategoriaID,
                CategoriaNombre = p.Categoria != null ? p.Categoria.Nombre : null,
                FotoUrl = p.Foto,
                Activo = p.Activo
            }).ToList();
        }

        public ProductoDTO? GetDTO(int id)
        {
            var p = _context.Productos.Include(x => x.Categoria).FirstOrDefault(x => x.ProductoID == id);
            if (p == null) return null;
            return new ProductoDTO
            {
                ProductoID = p.ProductoID,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Stock = p.Stock,
                CategoriaID = p.CategoriaID,
                CategoriaNombre = p.Categoria != null ? p.Categoria.Nombre : null,
                FotoUrl = p.Foto,
                Activo = p.Activo
            };
        }
    }
}
