using System;

namespace StockLine_API.DTOs
{
    public class MovimientoStockCreateResponseDTO
    {
        public int MovimientoID { get; set; }
        public DateTime Fecha { get; set; }
        public int ProductoID { get; set; }
        public string ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public string TipoMovimiento { get; set; }
        public int UsuarioID { get; set; }
        public string UsuarioNombre { get; set; }
        public string? Observaciones { get; set; }
        public int StockAfter { get; set; }
    }
}
