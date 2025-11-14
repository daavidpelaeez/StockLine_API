using Microsoft.EntityFrameworkCore;
using StockLine_API;
using StockLine_API.DTOs;
using StockLine_API.Models;
using StockLine_API.Services;

public class EnvioService
{
    private readonly StockLineContext _context;
    private readonly MovimientoStockService _movService;


    private static readonly string[] EstadosValidos = { "Pendiente", "Preparado", "Enviado", "Archivado" };

    public EnvioService(StockLineContext context, MovimientoStockService movService)
    {
        _context = context;
        _movService = movService;
    }


    public List<EnvioDTO> GetAll()
    {
        return _context.Envios
            .Include(e => e.Detalles)
                .ThenInclude(d => d.Producto)
            .Include(e => e.Detalles)
                .ThenInclude(d => d.SIM)
            .Include(e => e.Ayuntamiento)
            .Include(e => e.Comercial)
            .Include(e => e.UsuarioModificador)
            .Select(e => new EnvioDTO
            {
                EnvioID = e.EnvioID,
                AyuntamientoID = e.AyuntamientoID,
                AyuntamientoNombre = e.Ayuntamiento.Nombre,
                ComercialID = e.ComercialID,
                ComercialNombre = e.Comercial.Nombre + " " + e.Comercial.Apellidos,
                UsuarioModificadorID = e.UsuarioModificadorID,
                UsuarioModificadorNombre = e.UsuarioModificador != null 
                    ? e.UsuarioModificador.Nombre + " " + e.UsuarioModificador.Apellidos 
                    : null,
                NumeroReferencia = e.NumeroReferencia,
                Estado = e.Estado,
                FechaEnvio = e.FechaEnvio,
                FechaModificacion = e.FechaModificacion,
                Detalles = e.Detalles.Select(d => new EnvioDetalleDTO
                {
                    EnvioDetalleID = d.EnvioDetalleID,
                    ProductoID = d.ProductoID,
                    ProductoNombre = d.Producto.Nombre,
                    Cantidad = d.Cantidad,
                    SIMID = d.SIMID,
                    SIMNumero = d.SIM != null ? d.SIM.NumeroSIM : null
                }).ToList()
            }).ToList();
    }


    public EnvioDTO Get(int id)
    {
        var e = _context.Envios
            .Include(x => x.Detalles)
                .ThenInclude(d => d.Producto)
            .Include(x => x.Detalles)
                .ThenInclude(d => d.SIM)
            .Include(x => x.Ayuntamiento)
            .Include(x => x.Comercial)
            .Include(x => x.UsuarioModificador)
            .FirstOrDefault(x => x.EnvioID == id);

        if (e == null) return null;

        return new EnvioDTO
        {
            EnvioID = e.EnvioID,
            AyuntamientoID = e.AyuntamientoID,
            AyuntamientoNombre = e.Ayuntamiento.Nombre,
            ComercialID = e.ComercialID,
            ComercialNombre = e.Comercial.Nombre + " " + e.Comercial.Apellidos,
            UsuarioModificadorID = e.UsuarioModificadorID,
            UsuarioModificadorNombre = e.UsuarioModificador != null 
                ? e.UsuarioModificador.Nombre + " " + e.UsuarioModificador.Apellidos 
                : null,
            NumeroReferencia = e.NumeroReferencia,
            Estado = e.Estado,
            FechaEnvio = e.FechaEnvio,
            FechaModificacion = e.FechaModificacion,
            Detalles = e.Detalles.Select(d => new EnvioDetalleDTO
            {
                EnvioDetalleID = d.EnvioDetalleID,
                ProductoID = d.ProductoID,
                ProductoNombre = d.Producto.Nombre,
                Cantidad = d.Cantidad,
                SIMID = d.SIMID,
                SIMNumero = d.SIM != null ? d.SIM.NumeroSIM : null
            }).ToList()
        };
    }


    public EnvioDTO Create(CrearEnvioDTO dto, int usuarioID)
    {
        using var tx = _context.Database.BeginTransaction();

        try
        {
            var envio = new Envio
            {
                AyuntamientoID = dto.AyuntamientoID,
                ComercialID = dto.ComercialID,
                NumeroReferencia = dto.NumeroReferencia,
                Estado = "Pendiente",
                FechaEnvio = DateTime.Now,
                UsuarioModificadorID = usuarioID
            };

            _context.Envios.Add(envio);
            _context.SaveChanges();

            foreach (var p in dto.Productos)
            {
                var detalle = new EnvioDetalle
                {
                    EnvioID = envio.EnvioID,
                    ProductoID = p.ProductoID,
                    Cantidad = p.Cantidad,
                    SIMID = p.SIMID
                };
                _context.EnviosDetalle.Add(detalle);


                var producto = _context.Productos.FirstOrDefault(x => x.ProductoID == p.ProductoID);
                if (producto != null)
                {
                    producto.Stock -= p.Cantidad;
                    _context.SaveChanges();

                    // Crear movimiento negativo
                    _movService.Create(new MovimientoStock
                    {
                        ProductoID = producto.ProductoID,
                        Cantidad = -p.Cantidad,
                        TipoMovimiento = "Salida",
                        UsuarioID = usuarioID,
                        Observaciones = $"Envio {envio.EnvioID} - {envio.NumeroReferencia}"
                    });
                }
            }

            tx.Commit();

            return Get(envio.EnvioID);
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }


    public void UpdateEstado(int envioId, string estado, int usuarioID)
    {

        if (!EstadosValidos.Contains(estado))
        {
            throw new ArgumentException($"Estado inválido. Los estados válidos son: {string.Join(", ", EstadosValidos)}");
        }

        var e = _context.Envios
            .Include(x => x.Detalles)
            .Include(x => x.Ayuntamiento)
            .FirstOrDefault(x => x.EnvioID == envioId);
        if (e != null)
        {
            e.Estado = estado;
            e.UsuarioModificadorID = usuarioID;
            e.FechaModificacion = DateTime.Now;
            
            e.Ubicacion = e.Ayuntamiento != null ? e.Ayuntamiento.Nombre : "";
            _context.SaveChanges();

            
            if (estado == "Enviado")
            {
                foreach (var detalle in e.Detalles)
                {
                    
                    _movService.Create(new MovimientoStockDTO
                    {
                        ProductoID = detalle.ProductoID,
                        Cantidad = detalle.Cantidad,
                        TipoMovimiento = "Salida",
                        UsuarioID = usuarioID,
                        Observaciones = $"Envio {e.EnvioID} - {e.NumeroReferencia}"
                    });

                    
                    if (detalle.SIMID != null)
                    {
                        var sim = _context.SIMs.Find(detalle.SIMID);
                        if (sim != null)
                        {
                            sim.Ubicacion = e.Ubicacion ?? "En almacén";
                            _context.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
