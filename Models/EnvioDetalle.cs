namespace StockLine_API.Models
{
    public class EnvioDetalle
    {
        public int EnvioDetalleID { get; set; }
        public int EnvioID { get; set; }
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public int? SIMID { get; set; }
        public Envio Envio { get; set; }
        public Producto Producto { get; set; }
        public SIM? SIM { get; set; }
    }
}
