namespace WebProyecto.Models
{
    public class Direccion_Persona
    {
        public int iddireccion { get; set; }
        public string calle { get; set; }
        public string carrera { get; set; }
        public string ciudad { get; set; }
        public string barrio { get; set; }
        Persona fk_Direccion_Persona { get; set; }
    }
}
