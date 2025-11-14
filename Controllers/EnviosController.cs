using Microsoft.AspNetCore.Mvc;
using StockLine_API.DTOs;

[ApiController]
[Route("api/[controller]")]
public class EnviosController : ControllerBase
{
    private readonly EnvioService _service;

    public EnviosController(EnvioService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<List<EnvioDTO>> GetAll()
    {
        return _service.GetAll();
    }

    [HttpGet("{id}")]
    public ActionResult<EnvioDTO> Get(int id)
    {
        var envio = _service.Get(id);
        if (envio == null) return NotFound();
        return envio;
    }

    [HttpPost]
    public ActionResult<EnvioDTO> Create(CrearEnvioDTO dto, [FromQuery] int? usuarioModificadorId)
    {
        
        int usuarioID = usuarioModificadorId ?? 1;
        var envio = _service.Create(dto, usuarioID);
        return CreatedAtAction(nameof(Get), new { id = envio.EnvioID }, envio);
    }

    [HttpPatch("{id}/estado")]
    public IActionResult UpdateEstado(int id, [FromBody] string estado, [FromQuery] int? usuarioModificadorId)
    {
        try
        {
            
            int usuarioID = usuarioModificadorId ?? 1;
            _service.UpdateEstado(id, estado, usuarioID);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
