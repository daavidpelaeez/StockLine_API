using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockLine_API.Models;
using StockLine_API.DTOs;

namespace StockLine_API.Services
{
    public class SIMService
    {
        private readonly StockLineContext _context;
        public SIMService(StockLineContext context) => _context = context;

        public async Task<List<SIMDTO>> GetAllAsync()
        {
            var sims = await _context.SIMs.Include(s => s.Producto).ToListAsync();
            var simDtos = sims.Select(sim => new SIMDTO
            {
                SIMID = sim.SIMID,
                NumeroSIM = sim.NumeroSIM,
                ProductoID = sim.ProductoID,
                ProductoNombre = sim.Producto?.Nombre,
                Ubicacion = ObtenerUbicacionSIM(sim.SIMID),
                Estado = sim.ProductoID.HasValue ? "Asignada" : "Disponible",
                FechaAsignacion = sim.FechaAsignacion
            }).ToList();
            return simDtos;
        }

        public List<SIM> GetAll()
        {
            var sims = _context.SIMs.Include(s => s.Producto).ToList();
            foreach (var sim in sims)
            {
                sim.Ubicacion = ObtenerUbicacionSIM(sim.SIMID);
                sim.Estado = sim.ProductoID.HasValue ? "Asignada" : "Disponible";
            }
            return sims;
        }

        public List<SIM> GetByProducto(int productoId) =>
            _context.SIMs.Where(s => s.ProductoID == productoId).ToList();

        public List<SIM> GetSinProducto() =>
            _context.SIMs.Where(s => s.ProductoID == null).ToList();

        public SIM? Get(int id) =>
            _context.SIMs.FirstOrDefault(s => s.SIMID == id);

        public SIM Create(SIM s)
        {
            _context.SIMs.Add(s);
            _context.SaveChanges();
            return s;
        }

        public void Update(SIM s)
        {
            var sim = _context.SIMs.Find(s.SIMID);
            if (sim == null) return;
            sim.NumeroSIM = s.NumeroSIM;
            sim.ProductoID = s.ProductoID;
            _context.SaveChanges();
        }

        public void AsignarProducto(int simId, int? productoId)
        {
            var sim = _context.SIMs.Find(simId);
            if (sim == null) return;
            sim.ProductoID = productoId;
            sim.FechaAsignacion = DateTime.Now;
            
            if (!productoId.HasValue)
            {
                sim.Ubicacion = "En almacén";
            }
            else
            {
                
                sim.Ubicacion = ObtenerUbicacionSIM(sim.SIMID);
            }
            _context.SaveChanges();
        }

        public bool Delete(int id)
        {
            var sim = _context.SIMs.Find(id);
            if (sim == null) return false;

            
            var detalles = _context.EnviosDetalle.Where(ed => ed.SIMID == id).ToList();
            foreach (var detalle in detalles)
            {
                detalle.SIMID = null;
            }
            _context.SaveChanges();

            _context.SIMs.Remove(sim);
            _context.SaveChanges();
            return true;
        }

        
        private string ObtenerUbicacionSIM(int simId)
        {
            var sim = _context.SIMs.Find(simId);
            if (sim == null || sim.ProductoID == null)
            {
                return "En almacén";
            }
            var fechaAsignacion = sim.FechaAsignacion ?? DateTime.MinValue;
            var ultimoEnvio = _context.EnviosDetalle
                .Include(ed => ed.Envio)
                .Where(ed => ed.SIMID == simId && ed.Envio != null && ed.Envio.Estado == "Enviado" && ed.Envio.FechaEnvio > fechaAsignacion)
                .OrderByDescending(ed => ed.Envio.FechaEnvio)
                .FirstOrDefault();
            return (ultimoEnvio?.Envio?.Ubicacion) ?? "En almacén";
        }
    }
}
