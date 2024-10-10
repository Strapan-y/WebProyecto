namespace WebProyecto.Models
{
    public class Persona
    {
        public int idPersona { get; set; }
        public string? usu { get; set; }
        public string? contra { get; set; }
        public string? nombre { get; set; }
        public string? correo { get; set; }
        public string? apellido { get; set; }
        public string? telefono { get; set; }
        public tipoDocumento tipoDocumento { get; set; }
        public Usuario usuario { get; set; }
        public Direccion_Persona direccion { get; set; }

    }
}
