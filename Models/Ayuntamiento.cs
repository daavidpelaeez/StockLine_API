using System;

namespace StockLine_API.Models
{
    public class Ayuntamiento
    {
        public int AyuntamientoID { get; set; }
        public string Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Provincia { get; set; }
        public string? CP { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public int? ComercialID { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool Activo { get; set; } = true;
        public Comercial? Comercial { get; set; }
        public List<Envio> Envios { get; set; } = new();
    }
}
