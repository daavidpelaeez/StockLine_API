using System.Collections.Generic;
using System.Linq;
using StockLine_API.Models;

namespace StockLine_API.Services
{
    public class ComercialService
    {
        private readonly StockLineContext _context;
        public ComercialService(StockLineContext context) { _context = context; }
        public List<Comercial> GetAll() => _context.Comerciales.ToList();
        public Comercial Get(int id) => _context.Comerciales.FirstOrDefault(c => c.ComercialID == id);
        public Comercial Create(Comercial c) { _context.Comerciales.Add(c); _context.SaveChanges(); return c; }
        public void Update(Comercial c) { var e = _context.Comerciales.Find(c.ComercialID); if (e != null) { e.Nombre = c.Nombre; e.Apellidos = c.Apellidos; e.Email = c.Email; e.Telefono = c.Telefono; _context.SaveChanges(); } }
        public void Delete(int id) { var e = _context.Comerciales.Find(id); if (e != null) { _context.Comerciales.Remove(e); _context.SaveChanges(); } }
    }
}
