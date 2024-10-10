using Org.BouncyCastle.Asn1.Cms;

namespace WebProyecto.Models

{
    public class Agenda
    {
        public string? Id { get; set; }
        public string? fecha { get; set; }
        public DateTime? fechamuestra { get; set; }
        public string? duracion { get; set; }
        public string? empleado { get; set; }
        public string? lugar { get; set; }
        public string? hora { get; set;}
        public DateTime? horaM { get; set;}
        public string? encargado { get; set;}
        public int? usuario { get; set;}
        public int? reserva { get; set;}
        public Boolean? estado { get; set; }
    }
    
    

}
