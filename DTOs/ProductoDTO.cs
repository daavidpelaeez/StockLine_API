namespace StockLine_API.DTOs
{
    public class ProductoDTO
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
        public string? FotoUrl { get; set; }

        public int? CategoriaID { get; set; }       
        public string? CategoriaNombre { get; set; } 
        public bool Activo { get; set; } = true;
    }
}
