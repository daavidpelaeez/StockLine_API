using Microsoft.AspNetCore.Mvc;
using StockLine_API.DTOs;
using StockLine_API.Models;
using StockLine_API.Services;

namespace StockLine_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] CrearUsuarioDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Datos de usuario inválidos" });

            var user = new Usuario
            {
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                Email = dto.Email,
                RoleID = dto.RoleID
            };

            var created = _auth.Register(user, dto.Password);

            if (created == null)
                return BadRequest(new { message = "Usuario ya existe" });

            
            return Ok(new
            {
                message = "Usuario registrado",
                user = new
                {
                    created.UsuarioID,
                    created.Nombre,
                    created.Apellidos,
                    created.Email,
                    created.RoleID
                }
            });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Datos de login inválidos" });

            var user = _auth.Login(dto.Email, dto.Password);

            if (user == null)
                return Unauthorized(new { message = "Credenciales incorrectas" });

            return Ok(new
            {
                message = "Login correcto",
                user = new
                {
                    user.UsuarioID,
                    user.Nombre,
                    user.Apellidos,
                    user.Email,
                    user.RoleID
                }
            });
        }
    }
}
