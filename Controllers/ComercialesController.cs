using Microsoft.AspNetCore.Mvc;
using StockLine_API.Services;
using StockLine_API.DTOs;
using StockLine_API.Models;
using System.Collections.Generic;

namespace StockLine_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComercialesController : ControllerBase
    {
        private readonly ComercialService _service;

        public ComercialesController(ComercialService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<Comercial>> Get()
        {
            return _service.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<Comercial> Get(int id)
        {
            var c = _service.Get(id);
            if (c == null) return NotFound();
            return c;
        }

        [HttpPost]
        public ActionResult<Comercial> Create([FromBody] ComercialDTO dto)
        {
            var c = new Comercial
            {
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                Email = dto.Email,
                Telefono = dto.Telefono
            };
            _service.Create(c);
            return Ok(c);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ComercialDTO dto)
        {
            var c = _service.Get(id);
            if (c == null) return NotFound();

            c.Nombre = dto.Nombre;
            c.Apellidos = dto.Apellidos;
            c.Email = dto.Email;
            c.Telefono = dto.Telefono;

            _service.Update(c);
            return Ok(c);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return Ok(new { message = "Comercial eliminado" });
        }
    }
}
