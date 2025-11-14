namespace StockLine_API.DTOs
{
    public class MovimientoStockDTO
    {
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public string TipoMovimiento { get; set; }
        public int UsuarioID { get; set; }
        public string Observaciones { get; set; }
    }
}
