namespace WebProyecto.Models
{
    public class ConsutlaHistorialServicios
    {
        public string? id_reserva { get; set; }
        public string? fechainicio_age { get; set; }
        public string? horainicio_age { get; set; }
        public string? numLocal { get; set; }
        public string? descripcionlocal { get; set; }
        public string? Nombre_Servicio { get; set; }
        public string? nombre_persona { get; set; }
        public string? apellido_persona { get; set; }
        public string? Descripcion_Servicio { get; set; }
        public Usuario usuario { get; set; }
        public Persona per { get; set; }

    }
}
