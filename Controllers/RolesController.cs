using Microsoft.AspNetCore.Mvc;
using StockLine_API.Services;
using StockLine_API.DTOs;
using StockLine_API.Models;
using System.Collections.Generic;

namespace StockLine_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RoleService _service;

        public RolesController(RoleService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<Role>> Get()
        {
            var roles = _service.GetAll();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public ActionResult<Role> Get(int id)
        {
            var role = _service.Get(id);
            if (role == null) return NotFound(new { message = "Role no encontrado" });
            return Ok(role);
        }

        [HttpPost]
        public IActionResult Create([FromBody] RoleDTO dto)
        {
            if (dto == null) return BadRequest(new { message = "Datos inválidos" });

            var role = new Role
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };

            _service.Create(role);
            return Ok(new { message = "Role creado", role });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] RoleDTO dto)
        {
            if (dto == null) return BadRequest(new { message = "Datos inválidos" });

            var role = _service.Get(id);
            if (role == null) return NotFound(new { message = "Role no encontrado" });

            role.Nombre = dto.Nombre;
            role.Descripcion = dto.Descripcion;

            _service.Update(role);
            return Ok(new { message = "Role actualizado", role });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var role = _service.Get(id);
            if (role == null) return NotFound(new { message = "Role no encontrado" });

            _service.Delete(id);
            return Ok(new { message = "Role eliminado", roleId = id });
        }
    }
}
