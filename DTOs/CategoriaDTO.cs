namespace StockLine_API.DTOs
{
    public class CategoriaDTO
    {
        public int CategoriaID { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; } = true;
    }
}
