using System.Collections.Generic;
using System.Linq;
using StockLine_API.Models;
using Microsoft.EntityFrameworkCore;

namespace StockLine_API.Services
{
    public class UsuarioService
    {
        private readonly StockLineContext _context;

        public UsuarioService(StockLineContext context)
        {
            _context = context;
        }

        public List<Usuario> GetAll(bool? activos = true)
        {
            var query = _context.Usuarios.Include(u => u.Role).AsQueryable();
            if (activos == true)
                query = query.Where(u => u.Activo);
            else if (activos == false)
                query = query.Where(u => !u.Activo);
            return query.ToList();
        }
        
        public Usuario Get(int id) => _context.Usuarios
            .Include(u => u.Role)
            .FirstOrDefault(u => u.UsuarioID == id);
        
        public Usuario Create(Usuario u) 
        { 
            _context.Usuarios.Add(u); 
            _context.SaveChanges(); 
            return u; 
        }
        
        public void Update(Usuario u) 
        { 
            var e = _context.Usuarios.Find(u.UsuarioID); 
            if (e != null) 
            { 
                e.Nombre = u.Nombre; 
                e.Apellidos = u.Apellidos; 
                e.Email = u.Email; 
                if (!string.IsNullOrEmpty(u.PasswordHash)) 
                    e.PasswordHash = u.PasswordHash; 
                e.RoleID = u.RoleID; 
                e.Activo = u.Activo;
                _context.SaveChanges(); 
            } 
        }
        
        public bool SoftDelete(int id) 
        { 
            var usuario = _context.Usuarios.Find(id); 
            if (usuario == null) 
                return false;

            
            var tieneMovimientos = _context.MovimientosStock
                .Any(m => m.UsuarioID == id);

            if (tieneMovimientos)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el usuario porque tiene movimientos de stock asociados. " +
                    "Considere desactivar el usuario en lugar de eliminarlo.");
            }

            
            var tieneEnvios = _context.Envios
                .Any(e => e.UsuarioModificadorID == id);

            if (tieneEnvios)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el usuario porque tiene envíos asociados como modificador. " +
                    "Considere desactivar el usuario en lugar de eliminarlo.");
            }

            usuario.Activo = false;
            _context.SaveChanges();
            return true;
        }
    }
}
