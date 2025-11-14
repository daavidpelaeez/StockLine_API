namespace StockLine_API.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        public List<Usuario> Usuarios { get; set; } = new();
    }
}
