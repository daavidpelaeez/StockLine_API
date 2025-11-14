namespace StockLine_API.Models
{
    public class Categoria
    {
        public int CategoriaID { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; } = true;
        public ICollection<Producto>? Productos { get; set; }
    }
}
