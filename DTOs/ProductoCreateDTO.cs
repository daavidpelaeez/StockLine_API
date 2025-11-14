using Microsoft.AspNetCore.Http;

namespace StockLine_API.DTOs
{
    public class ProductoCreateDTO
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int Stock { get; set; }
        public int? CategoriaID { get; set; }
        public IFormFile? Foto { get; set; }
    }
}