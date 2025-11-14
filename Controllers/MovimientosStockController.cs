using Microsoft.AspNetCore.Mvc;
using StockLine_API.DTOs;
using StockLine_API.Services;
using System;
using System.Globalization;
using System.Linq;

namespace StockLine_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientosStockController : ControllerBase
    {
        private readonly MovimientoStockService _service;

        public MovimientosStockController(MovimientoStockService service)
        {
            _service = service;
        }

        
        [HttpGet]
        public ActionResult<List<MovimientoStockHistoryItemDTO>> Get(
            [FromQuery] int? productId,
            [FromQuery] int? usuarioId,
            [FromQuery] string? tipo,
            [FromQuery] string? from,
            [FromQuery] string? to,
            [FromQuery] string sortBy = "Fecha",
            [FromQuery] string sortDir = "desc")
        {
            DateTime? fromDt = TryParseDate(from);
            DateTime? toDt = TryParseDate(to);

            var result = _service.GetFiltered(productId, usuarioId, tipo, fromDt, toDt, sortBy, sortDir);
            return Ok(result);
        }

        
        [HttpGet("{id}")]
        public ActionResult<MovimientoStockHistoryItemDTO> GetById(int id)
        {
            var item = _service.GetById(id);
            if (item == null) return NotFound(new { message = "Movimiento no encontrado" });
            return Ok(item);
        }

        
        [HttpPost]
        public ActionResult<MovimientoStockCreateResponseDTO> Create([FromBody] MovimientoStockDTO dto)
        {
            if (dto == null) return BadRequest(new { message = "Datos inválidos" });
            try
            {
                var (movimiento, stockAfter) = _service.Create(dto);

                var response = new MovimientoStockCreateResponseDTO
                {
                    MovimientoID = movimiento.MovimientoID,
                    Fecha = movimiento.Fecha,
                    ProductoID = movimiento.ProductoID ?? 0, 
                    ProductoNombre = movimiento.Producto?.Nombre ?? string.Empty,
                    Cantidad = movimiento.Cantidad,
                    TipoMovimiento = movimiento.TipoMovimiento,
                    UsuarioID = movimiento.UsuarioID,
                    UsuarioNombre = movimiento.Usuario != null ? $"{movimiento.Usuario.Nombre} {movimiento.Usuario.Apellidos}" : string.Empty,
                    Observaciones = movimiento.Observaciones,
                    StockAfter = stockAfter
                };

                return CreatedAtAction(nameof(GetById), new { id = movimiento.MovimientoID }, response);
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(new { message = knf.Message });
            }
            catch (InvalidOperationException ioe)
            {
                return Conflict(new { message = ioe.Message });
            }
            catch (ArgumentException ae)
            {
                return BadRequest(new { message = ae.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno", detail = ex.Message });
            }
        }

        
        [HttpGet("summary")]
        public IActionResult Summary([FromQuery] int? productId, [FromQuery] string? from, [FromQuery] string? to)
        {
            var fromDt = TryParseDate(from);
            var toDt = TryParseDate(to);
            var res = _service.GetSummary(productId, fromDt, toDt);
            return Ok(res);
        }

        private static DateTime? TryParseDate(string? s)
        {
            if (string.IsNullOrEmpty(s)) return null;
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;
            if (DateTime.TryParse(s, out dt))
                return dt;
            return null;
        }
    }
}
