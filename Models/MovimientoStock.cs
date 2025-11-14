using System;

namespace StockLine_API.Models
{
    public class MovimientoStock
    {
        public int MovimientoID { get; set; }
        public int? ProductoID { get; set; }
        public int Cantidad { get; set; }
        public string TipoMovimiento { get; set; }
        public int UsuarioID { get; set; }
        public string Observaciones { get; set; }
        public DateTime Fecha { get; set; }
        public Producto Producto { get; set; }
        public Usuario Usuario { get; set; }
    }
}
