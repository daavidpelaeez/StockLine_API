using System.Linq;
using StockLine_API.Models;
using BCrypt.Net;

namespace StockLine_API.Services
{
    public class AuthService
    {
        private readonly StockLineContext _context;

        public AuthService(StockLineContext context)
        {
            _context = context;
        }

        public Usuario Register(Usuario user, string password)
        {
            var existing = _context.Usuarios.FirstOrDefault(u => u.Email == user.Email);
            if (existing != null) return null;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.Activo = true; 
            _context.Usuarios.Add(user);
            _context.SaveChanges();
            return user;
        }

        public Usuario Login(string email, string password)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.Email == email && u.Activo);
            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;
            return user;
        }
    }
}
