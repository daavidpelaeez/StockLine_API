using System;
using System.Collections.Generic;

namespace StockLine_API.Models
{
    public class Envio
    {
        public int EnvioID { get; set; }
        public int AyuntamientoID { get; set; }
        public int ComercialID { get; set; }
        public string NumeroReferencia { get; set; }
        public string Estado { get; set; }
        public DateTime FechaEnvio { get; set; }
        public int? UsuarioModificadorID { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string? Ubicacion { get; set; } 
        
        public Ayuntamiento Ayuntamiento { get; set; }
        public Comercial Comercial { get; set; }
        public Usuario? UsuarioModificador { get; set; }
        public List<EnvioDetalle> Detalles { get; set; } = new List<EnvioDetalle>();
    }
}
