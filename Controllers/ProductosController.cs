using Microsoft.AspNetCore.Mvc;
using StockLine_API.Models;
using StockLine_API.Services;
using StockLine_API.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace StockLine_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly ProductoService _service;

        public ProductosController(ProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<List<ProductoDTO>> Get()
        {
            var productos = _service.GetAll();

            var productosDto = productos.Select(p => new ProductoDTO
            {
                ProductoID = p.ProductoID,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Stock = p.Stock,
                CategoriaID = p.CategoriaID,
                CategoriaNombre = p.Categoria?.Nombre,
                FotoUrl = (p.Foto != null && p.Foto.Length > 0)
                    ? Url.Action("GetPhoto", "Productos", new { id = p.ProductoID }, Request.Scheme)
                    : null,
                Activo = p.Activo
            }).ToList();

            return Ok(productosDto);
        }

        [HttpGet("{id}")]
        public ActionResult<ProductoDTO> Get(int id)
        {
            var producto = _service.Get(id);
            if (producto == null) return NotFound(new { message = "Producto no encontrado" });
            
            var productoDto = new ProductoDTO
            {
                ProductoID = producto.ProductoID,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Stock = producto.Stock,
                CategoriaID = producto.CategoriaID,
                CategoriaNombre = producto.Categoria?.Nombre,
                FotoUrl = (producto.Foto != null && producto.Foto.Length > 0)
                    ? Url.Action("GetPhoto", "Productos", new { id = producto.ProductoID }, Request.Scheme)
                    : null,
                Activo = producto.Activo
            };
            
            return Ok(productoDto);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                var dto = new ProductoCreateDTO
                {
                    Nombre = form["Nombre"],
                    Descripcion = form["Descripcion"],
                    Stock = int.TryParse(form["Stock"], out var s) ? s : 0,
                    CategoriaID = int.TryParse(form["CategoriaID"], out var c) ? c : null,
                    Foto = form.Files["Foto"]
                };

                var producto = new Producto
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Stock = dto.Stock,
                    CategoriaID = dto.CategoriaID
                };
                _service.Create(producto);

                if (dto.Foto != null && dto.Foto.Length > 0)
                {
                    var extension = Path.GetExtension(dto.Foto.FileName);
                    var fileName = $"producto_{producto.ProductoID}_{DateTime.Now.Ticks}{extension}";
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "productos");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);
                    var filePath = Path.Combine(folderPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.Foto.CopyToAsync(stream);
                    }
                    producto.Foto = $"/images/productos/{fileName}";
                    _service.Update(producto);
                }

                var created = _service.Get(producto.ProductoID);
                var createdDto = new ProductoDTO
                {
                    ProductoID = created.ProductoID,
                    Nombre = created.Nombre,
                    Descripcion = created.Descripcion,
                    Stock = created.Stock,
                    CategoriaID = created.CategoriaID,
                    CategoriaNombre = created.Categoria?.Nombre,
                    FotoUrl = (created.Foto != null && created.Foto.Length > 0)
                        ? Url.Action("GetPhoto", "Productos", new { id = created.ProductoID }, Request.Scheme)
                        : null
                };
                return Ok(new { message = "Producto creado", producto = createdDto });
            }
            else if (Request.ContentType != null && Request.ContentType.Contains("application/json"))
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                var dto = JsonSerializer.Deserialize<ProductoDTO>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (dto == null) return BadRequest(new { message = "Datos inválidos" });

                var producto = new Producto
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Stock = dto.Stock,
                    CategoriaID = dto.CategoriaID
                };
                _service.Create(producto);

                var created = _service.Get(producto.ProductoID);
                var createdDto = new ProductoDTO
                {
                    ProductoID = created.ProductoID,
                    Nombre = created.Nombre,
                    Descripcion = created.Descripcion,
                    Stock = created.Stock,
                    CategoriaID = created.CategoriaID,
                    CategoriaNombre = created.Categoria?.Nombre,
                    FotoUrl = (created.Foto != null && created.Foto.Length > 0)
                        ? Url.Action("GetPhoto", "Productos", new { id = created.ProductoID }, Request.Scheme)
                        : null
                };
                return Ok(new { message = "Producto creado", producto = createdDto });
            }
            else
            {
                return BadRequest(new { message = "Tipo de contenido no soportado" });
            }
        }

        [HttpPost("upload/{id}")]
        public async Task<IActionResult> UploadPhoto(int id, [FromForm] ProductoFotoDTO dto)
        {
            if (dto.Foto == null || dto.Foto.Length == 0)
                return BadRequest(new { message = "No se envió ningún archivo" });

            var producto = _service.Get(id);
            if (producto == null)
                return NotFound(new { message = "Producto no encontrado" });

            var extension = Path.GetExtension(dto.Foto.FileName);
            var fileName = $"producto_{id}_{DateTime.Now.Ticks}{extension}";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "productos");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Foto.CopyToAsync(stream);
            }

            
            producto.Foto = $"/images/productos/{fileName}";
            _service.Update(producto);

            return Ok(new { message = "Foto subida", productoId = id, url = producto.Foto });
        }

        [HttpDelete("photo/{id}")]
        public IActionResult DeletePhoto(int id)
        {
            var producto = _service.Get(id);
            if (producto == null) return NotFound(new { message = "Producto no encontrado" });

            _service.RemovePhoto(id);
            return Ok(new { message = "FotoUrl eliminada", productoId = id });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProductoDTO dto)
        {
            if (dto == null) return BadRequest(new { message = "Datos inválidos" });

            var producto = _service.Get(id);
            if (producto == null) return NotFound(new { message = "Producto no encontrado" });

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.Stock = dto.Stock;
            producto.CategoriaID = dto.CategoriaID; 

            _service.Update(producto);

           
            var updated = _service.Get(id);
            var updatedDto = new ProductoDTO
            {
                ProductoID = updated.ProductoID,
                Nombre = updated.Nombre,
                Descripcion = updated.Descripcion,
                Stock = updated.Stock,
                CategoriaID = updated.CategoriaID,
                CategoriaNombre = updated.Categoria?.Nombre,
                FotoUrl = (updated.Foto != null && updated.Foto.Length > 0)
                    ? Url.Action("GetPhoto", "Productos", new { id = updated.ProductoID }, Request.Scheme)
                    : null
            };

            return Ok(new { message = "Producto actualizado", producto = updatedDto });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var producto = _service.Get(id);
            if (producto == null) return NotFound(new { message = "Producto no encontrado" });

            _service.Delete(id);
            return Ok(new { message = "Producto eliminado", productoId = id });
        }

        [HttpGet("photo/{id}")]
        public IActionResult GetPhoto(int id)
        {
            var producto = _service.Get(id);
            if (producto == null)
                return NotFound(new { message = "Producto no encontrado" });

            if (string.IsNullOrEmpty(producto.Foto))
                return NotFound(new { message = "Este producto no tiene foto" });

            
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", producto.Foto.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Archivo de foto no encontrado" });

            var contentType = "application/octet-stream";
            
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            if (ext == ".jpg" || ext == ".jpeg") contentType = "image/jpeg";
            if (ext == ".png") contentType = "image/png";

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType);
        }

        [HttpPut("{id}/categoria")]
        public IActionResult AsignarCategoria(int id, [FromBody] int categoriaId)
        {
            var producto = _service.Get(id);
            if (producto == null) return NotFound(new { message = "Producto no encontrado" });

            producto.CategoriaID = categoriaId;
            _service.Update(producto);

            return Ok(new { message = "Categoría asignada", productoId = id, categoriaId });
        }

        [HttpPut("{id}/stock")]
        public IActionResult UpdateStockManual(int id, [FromBody] int nuevoStock, [FromQuery] int usuarioID)
        {
            var producto = _service.Get(id);
            if (producto == null) return NotFound(new { message = "Producto no encontrado" });

            _service.UpdateStockManual(id, nuevoStock, usuarioID);

            var updated = _service.Get(id);
            var updatedDto = new ProductoDTO
            {
                ProductoID = updated.ProductoID,
                Nombre = updated.Nombre,
                Descripcion = updated.Descripcion,
                Stock = updated.Stock,
                CategoriaID = updated.CategoriaID,
                CategoriaNombre = updated.Categoria?.Nombre,
                FotoUrl = (updated.Foto != null && updated.Foto.Length > 0)
                    ? Url.Action("GetPhoto", "Productos", new { id = updated.ProductoID }, Request.Scheme)
                    : null,
                Activo = updated.Activo
            };

            return Ok(new { message = "Stock modificado manualmente", producto = updatedDto });
        }
    }
}
