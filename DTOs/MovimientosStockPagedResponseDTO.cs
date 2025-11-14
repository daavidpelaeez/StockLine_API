using System.Collections.Generic;

namespace StockLine_API.DTOs
{
    public class MovimientosStockPagedResponseDTO
    {
        public IEnumerable<MovimientoStockHistoryItemDTO> Items { get; set; } = new List<MovimientoStockHistoryItemDTO>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
