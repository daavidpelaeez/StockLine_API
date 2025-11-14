using StockLine_API.DTOs;
using System;
using System.ComponentModel.DataAnnotations;

namespace StockLine_API.Models
{
    public class Producto
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int Stock { get; set; }
        public String? Foto { get; set; }
        public int? CategoriaID { get; set; }
        public Categoria? Categoria { get; set; }
        public virtual ICollection<MovimientoStock> MovimientosStock { get; set; } = new List<MovimientoStock>();
        public virtual ICollection<EnvioDetalle> EnvioDetalles { get; set; } = new List<EnvioDetalle>();
        public bool Activo { get; set; } = true;
    }
}
