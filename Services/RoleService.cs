using System.Collections.Generic;
using System.Linq;
using StockLine_API.Models;

namespace StockLine_API.Services
{
    public class RoleService
    {
        private readonly StockLineContext _context;
        public RoleService(StockLineContext context) { _context = context; }
        public List<Role> GetAll() => _context.Roles.ToList();
        public Role Get(int id) => _context.Roles.FirstOrDefault(r => r.RoleID == id);
        public Role Create(Role r) { _context.Roles.Add(r); _context.SaveChanges(); return r; }
        public void Update(Role r) { var e = _context.Roles.Find(r.RoleID); if (e != null) { e.Nombre = r.Nombre; e.Descripcion = r.Descripcion; _context.SaveChanges(); } }
        public void Delete(int id) { var e = _context.Roles.Find(id); if (e != null) { _context.Roles.Remove(e); _context.SaveChanges(); } }
    }
}
