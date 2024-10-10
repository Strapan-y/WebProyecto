using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using WebProyecto.Data;
using WebProyecto.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


namespace WebProyecto.Controllers
{
    public class UsuarioController : Controller
    {

        CookieOptions coo = new CookieOptions();
        private readonly Contexto _contexto;

        public UsuarioController(Contexto contexto)
        {
            _contexto = contexto;
        }

        public IActionResult RegistrarPersona()
        {
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;

            DatosComboBox modelo = new DatosComboBox();
            var datos = modelo.ObtenerDatosDocumento();
            // Asegúrate de que la SelectList esté correctamente configurada.
            ViewBag.DatosComboBox = new SelectList(datos.Select(x => new { Value = x, Text = x }), "Value", "Text");
            return View();
        }

        [HttpPost]
        public IActionResult RegistrarPersona(Persona persona)
        {
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;
            MySqlConnection cone = new(_contexto.Conexion);
            cone.Open();

            MySqlCommand cmd = new("registro", cone);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_ID", persona.idPersona);
            cmd.Parameters.AddWithValue("@P_NOM", persona.nombre);
            cmd.Parameters.AddWithValue("@P_AP", persona.apellido);
            cmd.Parameters.AddWithValue("@P_NUMT", persona.telefono);
            cmd.Parameters.AddWithValue("@FK_TD", 1);

            MySqlCommand cmd1 = new("usuario", cone);
            cmd1.CommandType = System.Data.CommandType.StoredProcedure;
            cmd1.Parameters.AddWithValue("@usu", persona.usuario.correo);
            cmd1.Parameters.AddWithValue("@contra", persona.usuario.contrasena);
            cmd1.Parameters.AddWithValue("@fkPersona", persona.idPersona);


            cmd.ExecuteNonQuery();
            cmd1.ExecuteNonQuery();
            cone.Close();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;
            return View();
        }

        public IActionResult UsuarioMiagenda()
        {
            var idUsuario = HttpContext.Request.Cookies["idusuario"];
            if (idUsuario == null)
            {
                idUsuario = "";
            }
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true; 
            var listaServi = consultarHistorialAgendaActiva(idUsuario);
            return View(listaServi);
        }

        public List<ConsutlaHistorialServicios> consultarHistorialAgendaActiva(string usuario)
        {
            List<ConsutlaHistorialServicios> historialServicios = new List<ConsutlaHistorialServicios>();
            try
            {
                MySqlConnection cone = new(_contexto.Conexion);
                cone.Open();

                MySqlCommand cmd = new("consultarHistorialAgendaActiva", cone);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@usuario", usuario);

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ConsutlaHistorialServicios mas = new ConsutlaHistorialServicios();
                    mas.id_reserva = reader.GetString("id_reserva");
                    mas.fechainicio_age = reader.GetString("fechainicio_age").Substring(0, 10); ;
                    mas.numLocal = reader.GetString("nom_lugar");
                    mas.descripcionlocal = reader.GetString("direccion_lugar");
                    mas.horainicio_age = reader.GetString("horainicio_age");
                    mas.Nombre_Servicio = reader.GetString("Nombre_Servicio");
                    mas.nombre_persona = reader.GetString("nombre_persona");
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

        [HttpPost]
        public IActionResult CancelarAgenda(string idReserva)
        {
            MySqlConnection cone = new(_contexto.Conexion);
            cone.Open();
            MySqlCommand cmd = new("cancelarReserva", cone);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@idReserva", idReserva);

            cmd.ExecuteNonQuery();
            cone.Close();
            return RedirectToAction("UsuarioMiagenda", "Usuario");
        }

        
        public IActionResult UsuarioReservaServicios(string fecha,string hora)
        {

            List<Agenda> listaAgendasDi;
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;

            DatosComboBox modelo = new DatosComboBox();
            List<ComboBox> datos = modelo.ObtenerDatosTipoServicio();
            ViewBag.datosTipoServicio =
                new SelectList(datos.Select(x => new { Value = x.id, Text = $"{x.id}-{x.nombre} " }), "Value", "Text");

            if (!string.IsNullOrEmpty(fecha))
            {
                listaAgendasDi = listaAgendasDisponiblesEspecificas(fecha, hora);
                return View(listaAgendasDi);

            }
            else
            {
                listaAgendasDi = listaAgendasDisponibles();
                return View(listaAgendasDi);

            }
        }

        [HttpPost]
        public ActionResult Reservar(int servicioSeleccionado, Agenda agenda, string selectLugar)
        {
            var idUsuario = HttpContext.Request.Cookies["idusuario"];
            if (string.IsNullOrEmpty(idUsuario))
            {
                idUsuario = "0";
            }
            MySqlConnection cone = new(_contexto.Conexion);
            cone.Open();

            MySqlCommand cmd = new("generarReserva", cone);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fkServicio", selectLugar);
            cmd.Parameters.AddWithValue("@fkcliente", idUsuario);
            cmd.Parameters.AddWithValue("@fkidagenda", servicioSeleccionado);

            cmd.ExecuteNonQuery();
            cone.Close();
            return RedirectToAction("UsuarioMiagenda", "Usuario");

        }

        public List<Agenda> listaAgendasDisponiblesEspecificas(String fecha,String hora)
        {
            List<Agenda> agendaservicios = new List<Agenda>();
            try
            {
                using (MySqlConnection cone = new MySqlConnection(_contexto.Conexion))
                {
                    cone.Open();
                    MySqlCommand cmd = new("ConsultaAgendalibre", cone);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@hora", hora);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Agenda mas = new Agenda();
                            mas.Id = reader.GetString("idagenda");
                            mas.encargado = reader.GetString("nombre_persona");
                            mas.fecha = reader.GetString("fechainicio_age").Substring(0, 10);
                            mas.hora = reader.GetString("horainicio_age");
                            agendaservicios.Add(mas);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return agendaservicios;
        }



        public List<Agenda> listaAgendasDisponibles()
        {
            List<Agenda> agendaservicios = new List<Agenda>();
            try
            {
                using (MySqlConnection cone = new MySqlConnection(_contexto.Conexion))
                {
                    cone.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT agenda.idagenda,agenda.fechainicio_age,agenda.horainicio_age,persona.nombre_persona FROM agenda.agenda \r\ninner join agenda.usuario on agenda.fk_empleado=usuario.nombre_usuario\r\ninner join agenda.persona on usuario.FK_persona=persona.numdocu_persona\r\nWHERE estadoAgenda = 1;", cone);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Agenda mas = new Agenda();
                            mas.Id = reader.GetString("idagenda"); 
                            mas.encargado = reader.GetString("nombre_persona"); 
                            mas.fecha = reader.GetString("fechainicio_age").Substring(0,10); 
                            mas.hora = reader.GetString("horainicio_age"); 
                            agendaservicios.Add(mas);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return agendaservicios;
        }


        public IActionResult UsuarioHistorial()
        {
            var idUsuario = HttpContext.Request.Cookies["idusuario"];
            if (idUsuario == null)
            {
                idUsuario = "";
            }
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;
            var listaServi = consultarHistorialAgendaUsuario(idUsuario);
            return View(listaServi);
        }

        public List<ConsutlaHistorialServicios> consultarHistorialAgendaUsuario(string usuario)
        {
            List<ConsutlaHistorialServicios> historialServicios = new List<ConsutlaHistorialServicios>();
            try
            {
                MySqlConnection cone = new(_contexto.Conexion);
                cone.Open();
                MySqlCommand cmd = new("consultarHistorialAgendaUsuario", cone);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@usuario", usuario);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ConsutlaHistorialServicios mas = new ConsutlaHistorialServicios();
                    mas.id_reserva = reader.GetString("id_reserva");
                    mas.fechainicio_age = reader.GetString("fechainicio_age").Substring(0, 10); ;
                    mas.horainicio_age = reader.GetString("horainicio_age");
                    mas.Nombre_Servicio = reader.GetString("Nombre_Servicio");
                    mas.nombre_persona = reader.GetString("nombre_persona");
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

        public IActionResult Cerrar()
        {

            
            HttpContext.Response.Cookies.Delete("idusuario");
            HttpContext.Response.Cookies.Delete("tipoPersona");
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public IActionResult Index(Usuario usuario)
        {
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;
            Usuario user = new Usuario();

            if (User.Identity.IsAuthenticated)
            {
                // Eliminar cookies y limpiar sesión
                Cerrar();
            }
            try
            {
                using (MySqlConnection cone = new(_contexto.Conexion))
                {
                    cone.Open();
                    MySqlCommand cmd = new("consultarUsuario", cone);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usu", usuario.correo);
                    cmd.Parameters.AddWithValue("@con", usuario.contrasena);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        user = new Usuario
                        {
                            Id = reader.GetString("nombre_usuario"),
                            contrasena = reader.GetString("contra_usuario"),
                            tipo = reader.GetInt32("FK_ID_Rol")
                        };

                        HttpContext.Response.Cookies.Append("idusuario", reader.GetString("nombre_usuario").ToString(), coo);
                        HttpContext.Response.Cookies.Append("tipoPersona", reader.GetInt32("FK_ID_Rol").ToString(), coo);
                    }
                    else
                    {
                        user=null;
                    }
                    

                    cone.Close();
                }

            }
            catch (Exception)
            {

                throw;
            }

            if (user == null)
            {
                ModelState.AddModelError("", "ID de usuario o contraseña incorrectos.");
                return RedirectToAction("Index", "Usuario");
            }

            if (user.tipo>=3)
            {
                return RedirectToAction("Agenda", "Agenda");

            }else if (user.tipo >=2)
            {
                return RedirectToAction("Inicio", "Empleado");

            }
            else
            {
                return RedirectToAction("UsuarioMiagenda", "Usuario");

            }



        }
    }
}
