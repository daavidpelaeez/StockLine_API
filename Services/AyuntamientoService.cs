using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StockLine_API.Models;
using StockLine_API.DTOs;

namespace StockLine_API.Services
{
    public class AyuntamientoService
    {
        private readonly StockLineContext _context;
        public AyuntamientoService(StockLineContext context) { _context = context; }
        public List<Ayuntamiento> GetAll() => _context.Ayuntamientos.ToList();
        public Ayuntamiento Get(int id) => _context.Ayuntamientos.FirstOrDefault(a => a.AyuntamientoID == id);
        public Ayuntamiento Create(Ayuntamiento a) { _context.Ayuntamientos.Add(a); _context.SaveChanges(); return a; }
        
        public void Update(Ayuntamiento a) 
        { 
            var e = _context.Ayuntamientos.Find(a.AyuntamientoID); 
            if (e != null) 
            { 
                e.Nombre = a.Nombre; 
                e.Direccion = a.Direccion; 
                e.Ciudad = a.Ciudad; 
                e.Provincia = a.Provincia; 
                e.CP = a.CP; 
                e.Telefono = a.Telefono; 
                e.Email = a.Email; 
                e.ComercialID = a.ComercialID; 
                e.Activo = a.Activo; 
                _context.SaveChanges(); 
            } 
        }
        
        public (bool success, string message) Delete(int id) 
        { 
            var e = _context.Ayuntamientos
                .Include(a => a.Envios)
                .FirstOrDefault(a => a.AyuntamientoID == id); 
            
            if (e == null) 
            { 
                return (false, "Ayuntamiento no encontrado"); 
            }
            
            if (e.Envios != null && e.Envios.Any())
            {
                return (false, $"No se puede eliminar el ayuntamiento porque tiene {e.Envios.Count} envío(s) asociado(s)");
            }
            
            _context.Ayuntamientos.Remove(e); 
            _context.SaveChanges();
            return (true, "Ayuntamiento eliminado correctamente");
        }

        public List<AyuntamientoDTO> GetAllDTO()
        {
            return _context.Ayuntamientos.Select(a => new AyuntamientoDTO
            {
                AyuntamientoID = a.AyuntamientoID,
                Nombre = a.Nombre,
                Direccion = a.Direccion,
                Ciudad = a.Ciudad,
                Provincia = a.Provincia,
                CP = a.CP,
                Telefono = a.Telefono,
                Email = a.Email,
                ComercialID = a.ComercialID,
                Activo = a.Activo
            }).ToList();
        }

        public AyuntamientoDTO? GetDTO(int id)
        {
            var a = _context.Ayuntamientos.Find(id);
            if (a == null) return null;
            return new AyuntamientoDTO
            {
                AyuntamientoID = a.AyuntamientoID,
                Nombre = a.Nombre,
                Direccion = a.Direccion,
                Ciudad = a.Ciudad,
                Provincia = a.Provincia,
                CP = a.CP,
                Telefono = a.Telefono,
                Email = a.Email,
                ComercialID = a.ComercialID,
                Activo = a.Activo
            };
        }
    }
}
