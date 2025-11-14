using Microsoft.AspNetCore.Mvc;
using StockLine_API.Models;
using StockLine_API.Services;
using StockLine_API.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace StockLine_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SIMsController : ControllerBase
    {
        private readonly SIMService _service;

        public SIMsController(SIMService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<SIMDTO>> Get()
        {
            var sims = _service.GetAll();
            var dtos = sims.Select(s => new SIMDTO
            {
                SIMID = s.SIMID,
                NumeroSIM = s.NumeroSIM,
                ProductoID = s.ProductoID,
                ProductoNombre = s.Producto != null ? s.Producto.Nombre : null,
                Ubicacion = s.Ubicacion,
                Estado = s.Estado,
                FechaAsignacion = s.FechaAsignacion
            }).ToList();
            return Ok(dtos);
        }

        [HttpGet("producto/{productoId}")]
        public ActionResult<List<SIM>> GetByProducto(int productoId)
        {
            var sims = _service.GetByProducto(productoId);
            return Ok(sims);
        }

        [HttpGet("sin-producto")]
        public ActionResult<List<SIM>> GetSinProducto()
        {
            var sims = _service.GetSinProducto();
            return Ok(sims);
        }

        [HttpGet("{id}")]
        public ActionResult<SIMDTO> Get(int id)
        {
            var sim = _service.Get(id);
            if (sim == null) return NotFound(new { message = "SIM no encontrada" });
            var dto = new SIMDTO
            {
                SIMID = sim.SIMID,
                NumeroSIM = sim.NumeroSIM,
                ProductoID = sim.ProductoID,
                ProductoNombre = sim.Producto != null ? sim.Producto.Nombre : null,
                Ubicacion = sim.Ubicacion,
                Estado = sim.Estado,
                FechaAsignacion = sim.FechaAsignacion
            };
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] SIMDTO dto)
        {
            if (dto == null) return BadRequest(new { message = "Datos inválidos" });

            var sim = new SIM
            {
                NumeroSIM = dto.NumeroSIM,
                ProductoID = dto.ProductoID
            };

            _service.Create(sim);
            return Ok(new { message = "SIM creada", sim });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] SIMDTO dto)
        {
            if (dto == null) return BadRequest(new { message = "Datos inválidos" });

            var sim = _service.Get(id);
            if (sim == null) return NotFound(new { message = "SIM no encontrada" });

            sim.NumeroSIM = dto.NumeroSIM;
            sim.ProductoID = dto.ProductoID;

            _service.Update(sim);
            return Ok(new { message = "SIM actualizada", sim });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var sim = _service.Get(id);
            if (sim == null) return NotFound(new { message = "SIM no encontrada" });

            _service.Delete(id);
            return Ok(new { message = "SIM eliminada", simId = id });
        }

        [HttpPatch("{id}/asignar-producto")]
        public IActionResult AsignarProducto(int id, [FromBody] int? productoId)
        {
            var sim = _service.Get(id);
            if (sim == null) return NotFound(new { message = "SIM no encontrada" });

            _service.AsignarProducto(id, productoId);
            return Ok(new { message = "Producto asignado/desasignado", simId = id, productoId });
        }

        [HttpPut("{id}/desasignar")]
        public IActionResult DesasignarProducto(int id)
        {
            var sim = _service.Get(id);
            if (sim == null) return NotFound(new { message = "SIM no encontrada" });

            _service.AsignarProducto(id, null);
            return Ok(new { message = "SIM desasignada del producto", simId = id });
        }
    }
}
