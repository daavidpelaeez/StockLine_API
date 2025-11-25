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
        var strategy = _context.Database.CreateExecutionStrategy();
        return strategy.Execute(() =>
        {
            using var tx = _context.Database.BeginTransaction();
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioID == usuarioID);
                int? comercialId = usuario?.ComercialID;
                if (comercialId == null)
                {
                    if (dto.ComercialID > 0 && _context.Comerciales.Any(c => c.ComercialID == dto.ComercialID))
                    {
                        comercialId = dto.ComercialID;
                    }
                    else
                    {
                        throw new ArgumentException("No se ha especificado un comercial válido para el envío.");
                    }
                }

                var envio = new Envio
                {
                    AyuntamientoID = dto.AyuntamientoID,
                    ComercialID = comercialId.Value,
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
                }

                _context.SaveChanges();
                tx.Commit();

                return Get(envio.EnvioID);
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        });
    }


    public void UpdateEstado(int envioId, string estado, int usuarioID)
    {
        if (!EstadosValidos.Contains(estado))
        {
            throw new ArgumentException($"Estado inválido. Los estados válidos son: {string.Join(", ", EstadosValidos)}");
        }

        var strategy = _context.Database.CreateExecutionStrategy();
        strategy.Execute(() =>
        {
            using var tx = _context.Database.BeginTransaction();
            try
            {
                var e = _context.Envios
                    .Include(x => x.Detalles)
                    .ThenInclude(d => d.Producto)
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
                            string observacion = $"Envio {e.EnvioID} - {e.NumeroReferencia}";
                            _movService.Create(new MovimientoStockDTO
                            {
                                ProductoID = detalle.ProductoID,
                                Cantidad = detalle.Cantidad,
                                TipoMovimiento = "Salida",
                                UsuarioID = usuarioID,
                                Observaciones = observacion
                            });
                        }
                    }
                    tx.Commit();
                }
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        });
    }
}
