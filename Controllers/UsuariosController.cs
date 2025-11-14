using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StockLine_API.DTOs;
using StockLine_API.Models;
using StockLine_API.Services;

namespace StockLine_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _service;
        private readonly AuthService _authService;

        public UsuariosController(UsuarioService service, AuthService authService)
        {
            _service = service;
            _authService = authService;
        }

        
        [HttpGet]
        public ActionResult<List<UsuarioDTO>> Get([FromQuery] bool? activos = true)
        {
            var usuarios = _service.GetAll(activos);
            var dtos = usuarios.Select(u => new UsuarioDTO
            {
                UsuarioID = u.UsuarioID,
                Nombre = u.Nombre,
                Apellidos = u.Apellidos,
                Email = u.Email,
                RoleID = u.RoleID,
                Activo = u.Activo
            }).ToList();

            return Ok(dtos);
        }

        
        [HttpGet("{id}")]
        public ActionResult<UsuarioDTO> Get(int id)
        {
            var u = _service.Get(id);
            if (u == null) return NotFound(new { message = "Usuario no encontrado" });

            var dto = new UsuarioDTO
            {
                UsuarioID = u.UsuarioID,
                Nombre = u.Nombre,
                Apellidos = u.Apellidos,
                Email = u.Email,
                RoleID = u.RoleID,
                Activo = u.Activo
            };

            return Ok(dto);
        }

        
        [HttpPost]
        public IActionResult Create([FromBody] UsuarioDTO dto)
        {
            if (dto == null) return BadRequest(new { message = "Datos inválidos" });

            var existing = _service.GetAll().FirstOrDefault(u => u.Email == dto.Email);
            if (existing != null) return BadRequest(new { message = "Email ya registrado" });

            var u = new Usuario
            {
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                Email = dto.Email,
                RoleID = dto.RoleID,
                PasswordHash = "",
                Activo = dto.Activo 
            };

            _service.Create(u);
            return Ok(new { message = "Usuario creado", usuario = new UsuarioDTO { UsuarioID = u.UsuarioID, Nombre = u.Nombre, Apellidos = u.Apellidos, Email = u.Email, RoleID = u.RoleID, Activo = u.Activo } });
        }

        
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UsuarioDTO dto)
        {
            if (dto == null) return BadRequest(new { message = "Datos inválidos" });

            var u = _service.Get(id);
            if (u == null) return NotFound(new { message = "Usuario no encontrado" });

            u.Nombre = dto.Nombre;
            u.Apellidos = dto.Apellidos;
            u.Email = dto.Email;
            u.RoleID = dto.RoleID;
            u.Activo = dto.Activo; 

            _service.Update(u);

            var updatedDto = new UsuarioDTO
            {
                UsuarioID = u.UsuarioID,
                Nombre = u.Nombre,
                Apellidos = u.Apellidos,
                Email = u.Email,
                RoleID = u.RoleID,
                Activo = u.Activo
            };

            return Ok(new { message = "Usuario actualizado", usuario = updatedDto });
        }

        
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var u = _service.Get(id);
            if (u == null) return NotFound(new { message = "Usuario no encontrado" });

            try
            {
                var success = _service.SoftDelete(id);
                if (success)
                    return Ok(new { message = "Usuario desactivado", usuarioId = id });
                else
                    return NotFound(new { message = "Usuario no encontrado" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        
        [HttpPatch("{id}/password")]
        public IActionResult ChangePassword(int id, [FromBody] string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword)) return BadRequest(new { message = "Password inválido" });

            var u = _service.Get(id);
            if (u == null) return NotFound(new { message = "Usuario no encontrado" });

            u.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _service.Update(u);
            return Ok(new { message = "Password actualizado" });
        }
    }
}
