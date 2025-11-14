using Microsoft.AspNetCore.Mvc;
using StockLine_API.DTOs;
using StockLine_API.Services;

namespace StockLine_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly CategoriaService _service;

        public CategoriasController(CategoriaService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<CategoriaDTO>> Get()
        {
            var categorias = _service.GetAll();
            var dtos = categorias.Select(c => new CategoriaDTO
            {
                CategoriaID = c.CategoriaID,
                Nombre = c.Nombre,
                Activo = c.Activo
            }).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public ActionResult<CategoriaDTO> Get(int id)
        {
            var c = _service.Get(id);
            if (c == null) return NotFound(new { message = "Categoría no encontrada" });
            var categoria = _service.GetCategoriaModel(id);
            var dto = new CategoriaDTO
            {
                CategoriaID = categoria.CategoriaID,
                Nombre = categoria.Nombre,
                Activo = categoria.Activo
            };
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CategoriaDTO dto)
        {
            _service.Create(dto);
            return Ok(new { message = "Categoría creada" });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CategoriaDTO dto)
        {
            dto.CategoriaID = id;
            _service.Update(dto);
            return Ok(new { message = "Categoría actualizada" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return Ok(new { message = "Categoría eliminada" });
        }
    }
}
