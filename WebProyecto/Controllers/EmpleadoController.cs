using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebProyecto.Data;
using WebProyecto.Models;

namespace WebProyecto.Controllers
{
    public class EmpleadoController : Controller
    {

        private readonly Contexto _contexto;

        public EmpleadoController(Contexto contexto)
        {
            _contexto = contexto;
        }

       
        public IActionResult Inicio()
        {
            var idUsuario = HttpContext.Request.Cookies["idusuario"];
            if (idUsuario == null)
            {
                idUsuario = "";
            }
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;
            var listaServi = ConsultaHistorialAgendaEmpleado(idUsuario);
            return View(listaServi);
        }

        public List<ConsutlaHistorialServicios> ConsultaHistorialAgendaEmpleado(string usuario)
        {
            List<ConsutlaHistorialServicios> historialServicios = new List<ConsutlaHistorialServicios>();
            try
            {
                MySqlConnection cone = new(_contexto.Conexion);
                cone.Open();

                MySqlCommand cmd = new("ConsultaHistorialAgendaEmpleado", cone);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@usuario", usuario);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ConsutlaHistorialServicios mas = new ConsutlaHistorialServicios();
                    mas.id_reserva = reader.GetString("id_reserva");
                    mas.fechainicio_age = reader.GetString("fechainicio_age").Substring(0, 10); ;
                    mas.descripcionlocal = reader.GetString("direccion_lugar");
                    mas.horainicio_age = reader.GetString("horainicio_age");
                    mas.Nombre_Servicio = reader.GetString("Nombre_Servicio");
                    mas.nombre_persona= reader.GetString("nombre_persona");
                    mas.Descripcion_Servicio = reader.GetString("Descripcion_Servicio");
                    historialServicios.Add(mas);

                }
                cone.Close();
            }
            catch (Exception)
            {
            }
            return historialServicios;
        }
    }
}
