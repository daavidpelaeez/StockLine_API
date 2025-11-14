using System;

namespace StockLine_API.Models
{
    public class Comercial
    {
        public int ComercialID { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }

        public List<Envio> Envios { get; set; } = new();
    }
}
