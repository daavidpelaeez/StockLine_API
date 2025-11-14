using System;
using System.ComponentModel.DataAnnotations;

namespace StockLine_API.Models
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } 
        public int RoleID { get; set; }
        public Role Role { get; set; }
        public List<MovimientoStock> MovimientosStock { get; set; } = new List<MovimientoStock>();
        public bool Activo { get; set; } = true;
    }
}
