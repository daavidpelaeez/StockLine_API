using Microsoft.AspNetCore.Http;

namespace StockLine_API.DTOs
{
    public class ProductoUploadDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
        public IFormFile Foto { get; set; }
    }
}
