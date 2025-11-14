using Microsoft.AspNetCore.Mvc;
using StockLine_API.Services;
using StockLine_API.DTOs;
using StockLine_API.Models;
using System.Collections.Generic;
using System.Linq;

namespace StockLine_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AyuntamientosController : ControllerBase
    {
        private readonly AyuntamientoService _service;

        public AyuntamientosController(AyuntamientoService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<AyuntamientoDTO>> Get()
        {
            var ayuntamientos = _service.GetAll();
            var dtos = ayuntamientos.Select(a => new AyuntamientoDTO
            {
                AyuntamientoID = a.AyuntamientoID,
                Nombre = a.Nombre,
                Direccion = a.Direccion,
                Ciudad = a.Ciudad,
                Provincia = a.Provincia,
                CP = a.CP,
                Telefono = a.Telefono,
                Email = a.Email,
                ComercialID = a.ComercialID,
                Activo = a.Activo
            }).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public ActionResult<AyuntamientoDTO> Get(int id)
        {
            var a = _service.Get(id);
            if (a == null) return NotFound();
            var dto = new AyuntamientoDTO
            {
                AyuntamientoID = a.AyuntamientoID,
                Nombre = a.Nombre,
                Direccion = a.Direccion,
                Ciudad = a.Ciudad,
                Provincia = a.Provincia,
                CP = a.CP,
                Telefono = a.Telefono,
                Email = a.Email,
                ComercialID = a.ComercialID,
                Activo = a.Activo
            };
            return Ok(dto);
        }

        [HttpPost]
        public ActionResult<Ayuntamiento> Create([FromBody] AyuntamientoDTO dto)
        {
            var a = new Ayuntamiento
            {
                Nombre = dto.Nombre,
                Direccion = dto.Direccion,
                Ciudad = dto.Ciudad,
                Provincia = dto.Provincia,
                CP = dto.CP,
                Telefono = dto.Telefono,
                Email = dto.Email,
                ComercialID = dto.ComercialID
            };
            _service.Create(a);
            return Ok(a);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] AyuntamientoDTO dto)
        {
            var a = _service.Get(id);
            if (a == null) return NotFound();

            a.Nombre = dto.Nombre;
            a.Direccion = dto.Direccion;
            a.Ciudad = dto.Ciudad;
            a.Provincia = dto.Provincia;
            a.CP = dto.CP;
            a.Telefono = dto.Telefono;
            a.Email = dto.Email;
            a.ComercialID = dto.ComercialID;
            a.Activo = dto.Activo; 

            _service.Update(a);
            return Ok(a);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var (success, message) = _service.Delete(id);
            
            if (!success)
            {
                return BadRequest(new { message });
            }
            
            return Ok(new { message });
        }
    }
}
