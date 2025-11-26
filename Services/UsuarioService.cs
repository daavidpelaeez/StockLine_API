using System.Collections.Generic;
using System.Linq;
using StockLine_API.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace StockLine_API.Services
{
    public class UsuarioService
    {
        private readonly StockLineContext _context;
        private readonly ComercialService _comercialService;
        // Define aquí el ID del rol comercial (ajusta según tu base de datos)
        private const int ROLE_COMERCIAL_ID = 2; // Cambia este valor si tu ID de rol comercial es diferente

        public UsuarioService(StockLineContext context, ComercialService comercialService)
        {
            _context = context;
            _comercialService = comercialService;
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
        
        // Nuevo método para crear usuario y asociar comercial si corresponde
        public Usuario Create(Usuario u, string password) 
        { 
            // Verifica si ya existe el usuario
            var existing = _context.Usuarios.FirstOrDefault(x => x.Email == u.Email);
            if (existing != null) return null;

            u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            u.Activo = true;
            _context.Usuarios.Add(u); 
            _context.SaveChanges(); 

            // Si el usuario es comercial, crea el comercial y asocia
            if (u.RoleID == ROLE_COMERCIAL_ID)
            {
                var comercial = new Comercial
                {
                    Nombre = u.Nombre,
                    Apellidos = u.Apellidos,
                    Email = u.Email,
                    Telefono = "",
                };
                _comercialService.Create(comercial);
                u.ComercialID = comercial.ComercialID;
                _context.SaveChanges();
            }
            return u; 
        }
        
        public void Update(Usuario u) 
        { 
            var e = _context.Usuarios.Find(u.UsuarioID); 
            if (e != null) 
            { 
                // Validar que el RoleID existe
                var roleExists = _context.Roles.Any(r => r.RoleID == u.RoleID);
                if (!roleExists)
                    throw new InvalidOperationException($"El rol con ID {u.RoleID} no existe.");

                // Validar que el email no esté en uso por otro usuario
                var emailExists = _context.Usuarios.Any(x => x.Email == u.Email && x.UsuarioID != u.UsuarioID);
                if (emailExists)
                    throw new InvalidOperationException($"El email '{u.Email}' ya está en uso por otro usuario.");

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
