using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using System.Globalization;
using WebProyecto.Data;
using WebProyecto.Models;

namespace WebProyecto.Controllers
{
    public class AgendaController : Controller
    {
        private readonly Contexto _contexto;

        public AgendaController(Contexto contexto)
        {
            _contexto = contexto;
        }

        public IActionResult Agenda()
        {
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;
            DatosComboBox modelo = new DatosComboBox();
            List<Persona> datos = modelo.ObtenerDatos();
            List<Persona> datoslugar = modelo.ObtenerDatosLugar();
            

            ViewBag.DatosComboBoxPersonas = 
                new SelectList(datos.Select(x => new { Value = x.correo, Text = $"{x.correo}-{x.nombre} " }), "Value", "Text");

            // Crear SelectList
            ViewBag.DatosLugares =
                new SelectList(datoslugar.Select(x => new { Value = x.correo, Text = $"{x.correo}-{x.nombre} " }), "Value", "Text");

            return View();
        }

        public IActionResult AgendaLugar()
        {
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;
            var listaServi = consultarLugares();
            return View(listaServi);
        }


        public List<AgendaLugar> consultarLugares()
        {
            List<AgendaLugar> historialServicios = new List<AgendaLugar>();
            try
            {
                MySqlConnection cone = new(_contexto.Conexion);
                cone.Open();
                MySqlCommand cmd = new("consultaLugares", cone);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    AgendaLugar mas = new AgendaLugar();
                    mas.id = reader.GetString("idLugar");
                    mas.nombre = reader.GetString("nom_lugar");
                    mas.direccion = reader.GetString("direccion_lugar");
                    mas.numerolocal = reader.GetString("nomlocal_lugar");
                    mas.municipio = reader.GetString("nombre_municipio");
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
        public IActionResult AgendaLugar(AgendaLugar agendaLugar)
        {
            MySqlConnection cone = new(_contexto.Conexion);
            cone.Open();

            MySqlCommand cmd = new("lugar", cone);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nomlugar", agendaLugar.nombre);
            cmd.Parameters.AddWithValue("@dirlu", agendaLugar.direccion);
            cmd.Parameters.AddWithValue("@numlocal", agendaLugar.numerolocal);
            cmd.Parameters.AddWithValue("@fk_muni", 2);


            cmd.ExecuteNonQuery();
            cone.Close();
            return RedirectToAction("AgendaLugar", "Agenda");

        }

        public IActionResult AgendaServicio()
        {
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;
            var listaServi = listaServicio(); 
            return View(listaServi);
        }

        public List<Agendaservicio> listaServicio()
        {
            List<Agendaservicio> agendaservicios = new List<Agendaservicio>();
            try
            {
                MySqlConnection cone = new(_contexto.Conexion);
                cone.Open();
                MySqlCommand cmd = new("SELECT * FROM agenda.servicio where Estado_Servicio=1", cone);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Agendaservicio mas = new Agendaservicio();
                    mas.nombre = reader.GetString(1);
                    mas.descripcion = reader.GetString(2);
                    mas.tiempo = reader.GetString(4);
                    agendaservicios.Add(mas);

                }
                cone.Close();
            }
            catch (Exception)
            {
            }
            return agendaservicios;
        }

        [HttpPost]
        public IActionResult AgendaServicio(Agendaservicio agendaservicio)
        {

            MySqlConnection cone = new(_contexto.Conexion);
            cone.Open();

            MySqlCommand cmd = new("Servicio", cone);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nomservi", agendaservicio.nombre);
            cmd.Parameters.AddWithValue("@descri", agendaservicio.descripcion);
            cmd.Parameters.AddWithValue("@tiemp", agendaservicio.tiempo);


            cmd.ExecuteNonQuery();
            cone.Close();
            return RedirectToAction("AgendaServicio", "Agenda");
        }


        public IActionResult parametros_agenda()
        {
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;
            return View();
        }

        [HttpPost]
        public IActionResult parametros_agenda(parametros_agenda parametros_agenda)
        {
            try
            {
                using (MySqlConnection cone = new(_contexto.Conexion))
                {
                    cone.Open();
                    MySqlCommand cmd = new("parametros_agenda", cone);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    // Asignar valores a los parámetros del procedimiento almacenado
                    cmd.Parameters.AddWithValue("@fei", parametros_agenda.fechaInicio);
                    cmd.Parameters.AddWithValue("@ffin", parametros_agenda.fechaFin);
                    cmd.Parameters.AddWithValue("@hoi", parametros_agenda.horaInicio);
                    cmd.Parameters.AddWithValue("@hof", parametros_agenda.horaFin);
                    cmd.Parameters.AddWithValue("@dur", parametros_agenda.duracion);
                    cmd.Parameters.AddWithValue("@lun", parametros_agenda.lunes);
                    cmd.Parameters.AddWithValue("@mar",parametros_agenda.martes);
                    cmd.Parameters.AddWithValue("@mier", parametros_agenda.miercoles);
                    cmd.Parameters.AddWithValue("@juev", parametros_agenda.jueves);
                    cmd.Parameters.AddWithValue("@vier", parametros_agenda.viernes);
                    cmd.Parameters.AddWithValue("@sab", parametros_agenda.sabado);
                    cmd.Parameters.AddWithValue("@dom", parametros_agenda.domingo);
                    // ... otros parámetros ...

                    cmd.ExecuteNonQuery();
                    cone.Close();
                }
            }
            catch (Exception ex)
            {
            }

            // Redireccionar a la vista o acción deseada después de guardar los datos
            return RedirectToAction("Agenda", "Agenda");
        }

        [HttpPost]
        public IActionResult Agenda(parametros_agenda agenda,string selectPersona, string selectLugar)
        {
            
            // Parseo de las fechas y horas
            DateTime fechaInicio = DateTime.ParseExact(agenda.fechaInicio, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime fechaFin = DateTime.ParseExact(agenda.fechaFin, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            TimeSpan horaInicioDiaria = TimeSpan.Parse(agenda.horaInicio);
            TimeSpan horaFinDiaria = TimeSpan.Parse(agenda.horaFin);
            int duracion = int.Parse(agenda.duracion);

            int diaDeLaSemanaInicio = ((int)fechaInicio.DayOfWeek == 0) ? 7 : (int)fechaInicio.DayOfWeek;


            // Variables para los días seleccionados
            bool lunesSeleccionado = Request.Form["lunes"].FirstOrDefault() != null;
            bool martesSeleccionado = Request.Form["martes"].FirstOrDefault() != null;
            bool miercolesSeleccionado = Request.Form["miercoles"].FirstOrDefault() != null;
            bool juevesSeleccionado = Request.Form["jueves"].FirstOrDefault() != null;
            bool viernesSeleccionado = Request.Form["viernes"].FirstOrDefault() != null;
            bool sabadoSeleccionado = Request.Form["sabado"].FirstOrDefault() != null;
            bool domingoSeleccionado = Request.Form["domingo"].FirstOrDefault() != null;



            MySqlConnection cone = new(_contexto.Conexion);

            try
            {
                cone.Open();

                for (DateTime fechaActual = fechaInicio; fechaActual <= fechaFin; fechaActual = fechaActual.AddDays(1))
                {
                    TimeSpan horaActual = horaInicioDiaria;
                    diaDeLaSemanaInicio= diaDeLaSemanaInicio = ((int)fechaActual.DayOfWeek == 0) ? 7 : (int)fechaActual.DayOfWeek;


                    if (lunesSeleccionado && (diaDeLaSemanaInicio == agenda.lunes))
                    {
                        while (horaActual.Add(TimeSpan.FromMinutes(duracion)) <= horaFinDiaria)
                        {
                            DateTime startDateTime = fechaActual.Add(horaActual);
                            DateTime endDateTime = startDateTime.AddMinutes(duracion); 

                            MySqlCommand cmd = new MySqlCommand("agendar", cone);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@fei", startDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@ffin", endDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@hoi", startDateTime.TimeOfDay); // TimeSpan que representa la hora de inicio
                            cmd.Parameters.AddWithValue("@hof", endDateTime.TimeOfDay); // TimeSpan que representa la hora de fin
                            cmd.Parameters.AddWithValue("@dur", duracion);
                            cmd.Parameters.AddWithValue("@fk_empleado", selectPersona);
                            cmd.Parameters.AddWithValue("@fk_lugar", selectLugar);

                            cmd.ExecuteNonQuery();

                            // Incrementar la hora actual por la duración para el siguiente bloque
                            horaActual = horaActual.Add(TimeSpan.FromMinutes(duracion));
                        }
                    }
                    else if(martesSeleccionado && (diaDeLaSemanaInicio == agenda.martes))
                    {
                        while (horaActual.Add(TimeSpan.FromMinutes(duracion)) <= horaFinDiaria)
                        {
                            DateTime startDateTime = fechaActual.Add(horaActual);
                            DateTime endDateTime = startDateTime.AddMinutes(duracion); // Nota: Cambié a AddMinutes aquí si duracion está en minutos

                            MySqlCommand cmd = new MySqlCommand("agendar", cone);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@fei", startDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@ffin", endDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@hoi", startDateTime.TimeOfDay); // TimeSpan que representa la hora de inicio
                            cmd.Parameters.AddWithValue("@hof", endDateTime.TimeOfDay); // TimeSpan que representa la hora de fin
                            cmd.Parameters.AddWithValue("@dur", duracion);
                            cmd.Parameters.AddWithValue("@fk_empleado", selectPersona);
                            cmd.Parameters.AddWithValue("@fk_lugar", selectLugar);

                            cmd.ExecuteNonQuery();

                            // Incrementar la hora actual por la duración para el siguiente bloque
                            horaActual = horaActual.Add(TimeSpan.FromMinutes(duracion));
                        }

                    }
                    else if(miercolesSeleccionado && (diaDeLaSemanaInicio== agenda.miercoles))
                    {
                        while (horaActual.Add(TimeSpan.FromMinutes(duracion)) <= horaFinDiaria)
                        {
                            DateTime startDateTime = fechaActual.Add(horaActual);
                            DateTime endDateTime = startDateTime.AddMinutes(duracion); // Nota: Cambié a AddMinutes aquí si duracion está en minutos

                            MySqlCommand cmd = new MySqlCommand("agendar", cone);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@fei", startDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@ffin", endDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@hoi", startDateTime.TimeOfDay); // TimeSpan que representa la hora de inicio
                            cmd.Parameters.AddWithValue("@hof", endDateTime.TimeOfDay); // TimeSpan que representa la hora de fin
                            cmd.Parameters.AddWithValue("@dur", duracion);
                            cmd.Parameters.AddWithValue("@fk_empleado", selectPersona);
                            cmd.Parameters.AddWithValue("@fk_lugar", selectLugar);

                            cmd.ExecuteNonQuery();

                            // Incrementar la hora actual por la duración para el siguiente bloque
                            horaActual = horaActual.Add(TimeSpan.FromMinutes(duracion));
                        }

                    }
                    else if (juevesSeleccionado && (diaDeLaSemanaInicio == agenda.jueves))
                    {
                        while (horaActual.Add(TimeSpan.FromMinutes(duracion)) <= horaFinDiaria)
                        {
                            DateTime startDateTime = fechaActual.Add(horaActual);
                            DateTime endDateTime = startDateTime.AddMinutes(duracion); // Nota: Cambié a AddMinutes aquí si duracion está en minutos

                            MySqlCommand cmd = new MySqlCommand("agendar", cone);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@fei", startDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@ffin", endDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@hoi", startDateTime.TimeOfDay); // TimeSpan que representa la hora de inicio
                            cmd.Parameters.AddWithValue("@hof", endDateTime.TimeOfDay); // TimeSpan que representa la hora de fin
                            cmd.Parameters.AddWithValue("@dur", duracion);
                            cmd.Parameters.AddWithValue("@fk_empleado", selectPersona);
                            cmd.Parameters.AddWithValue("@fk_lugar", selectLugar);

                            cmd.ExecuteNonQuery();

                            // Incrementar la hora actual por la duración para el siguiente bloque
                            horaActual = horaActual.Add(TimeSpan.FromMinutes(duracion));
                        }

                    }
                    else if (viernesSeleccionado && (diaDeLaSemanaInicio == agenda.viernes))
                    {
                        while (horaActual.Add(TimeSpan.FromMinutes(duracion)) <= horaFinDiaria)
                        {
                            DateTime startDateTime = fechaActual.Add(horaActual);
                            DateTime endDateTime = startDateTime.AddMinutes(duracion); // Nota: Cambié a AddMinutes aquí si duracion está en minutos

                            MySqlCommand cmd = new MySqlCommand("agendar", cone);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@fei", startDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@ffin", endDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@hoi", startDateTime.TimeOfDay); // TimeSpan que representa la hora de inicio
                            cmd.Parameters.AddWithValue("@hof", endDateTime.TimeOfDay); // TimeSpan que representa la hora de fin
                            cmd.Parameters.AddWithValue("@dur", duracion);
                            cmd.Parameters.AddWithValue("@fk_empleado", selectPersona);
                            cmd.Parameters.AddWithValue("@fk_lugar", selectLugar);

                            cmd.ExecuteNonQuery();

                            // Incrementar la hora actual por la duración para el siguiente bloque
                            horaActual = horaActual.Add(TimeSpan.FromMinutes(duracion));
                        }

                    }
                    else if (sabadoSeleccionado && (diaDeLaSemanaInicio == agenda.sabado))
                    {
                        while (horaActual.Add(TimeSpan.FromMinutes(duracion)) <= horaFinDiaria)
                        {
                            DateTime startDateTime = fechaActual.Add(horaActual);
                            DateTime endDateTime = startDateTime.AddMinutes(duracion); // Nota: Cambié a AddMinutes aquí si duracion está en minutos

                            MySqlCommand cmd = new MySqlCommand("agendar", cone);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@fei", startDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@ffin", endDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@hoi", startDateTime.TimeOfDay); // TimeSpan que representa la hora de inicio
                            cmd.Parameters.AddWithValue("@hof", endDateTime.TimeOfDay); // TimeSpan que representa la hora de fin
                            cmd.Parameters.AddWithValue("@dur", duracion);
                            cmd.Parameters.AddWithValue("@fk_empleado", selectPersona);
                            cmd.Parameters.AddWithValue("@fk_lugar", selectLugar);

                            cmd.ExecuteNonQuery();

                            // Incrementar la hora actual por la duración para el siguiente bloque
                            horaActual = horaActual.Add(TimeSpan.FromMinutes(duracion));
                        }

                    }
                    else if (domingoSeleccionado && (diaDeLaSemanaInicio == agenda.domingo))
                    {
                        while (horaActual.Add(TimeSpan.FromMinutes(duracion)) <= horaFinDiaria)
                        {
                            DateTime startDateTime = fechaActual.Add(horaActual);
                            DateTime endDateTime = startDateTime.AddMinutes(duracion); // Nota: Cambié a AddMinutes aquí si duracion está en minutos

                            MySqlCommand cmd = new MySqlCommand("agendar", cone);
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@fei", startDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@ffin", endDateTime.Date); // Solo la fecha, sin la hora
                            cmd.Parameters.AddWithValue("@hoi", startDateTime.TimeOfDay); // TimeSpan que representa la hora de inicio
                            cmd.Parameters.AddWithValue("@hof", endDateTime.TimeOfDay); // TimeSpan que representa la hora de fin
                            cmd.Parameters.AddWithValue("@dur", duracion);
                            cmd.Parameters.AddWithValue("@fk_empleado", selectPersona);
                            cmd.Parameters.AddWithValue("@fk_lugar", selectLugar);

                            cmd.ExecuteNonQuery();

                            // Incrementar la hora actual por la duración para el siguiente bloque
                            horaActual = horaActual.Add(TimeSpan.FromMinutes(duracion));
                        }

                    }


                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción aquí
                // Log the exception or handle it as needed
            }
            finally
            {
                if (cone != null && cone.State == System.Data.ConnectionState.Open)
                {
                    cone.Close();
                }
            }

            return RedirectToAction("Agenda", "Agenda");

        }


        public IActionResult AgendaHistorial()
        {
            var idUsuario = HttpContext.Request.Cookies["idusuario"];
            if (idUsuario == null)
            {
                idUsuario = "";
            }
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;
            var listaServi = consultarHistorialAgenda();
            return View(listaServi);
        }

        public List<ConsutlaHistorialServicios> consultarHistorialAgenda()
        {
            List<ConsutlaHistorialServicios> historialServicios = new List<ConsutlaHistorialServicios>();
            try
            {
                MySqlConnection cone = new(_contexto.Conexion);
                cone.Open();
                MySqlCommand cmd = new("consultarHistorialAgendas", cone);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

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

        public IActionResult AgendaCrearUsuario()
        {
            
            ViewData["HideNavBar"] = true;
            ViewData["HideFooter"] = true;
            var listaServi = consultarUsuarios();
            DatosComboBox modelo = new DatosComboBox();
            List<Persona> datosTipoDocumento = modelo.ObtenerDatosTipoDocumento();

            ViewBag.DatosTipoDocumento =
                new SelectList(datosTipoDocumento.Select(x => new { Value = x.nombre, Text = $"{x.nombre}-{x.correo} " }), "Value", "Text");

            return View(listaServi);
        }

        [HttpPost]
        public IActionResult AgendaCrearUsuario(Persona persona, string selectTipoDocumento)
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
            cmd.Parameters.AddWithValue("@FK_TD", selectTipoDocumento);

            MySqlCommand cmd1 = new("usuario", cone);
            cmd1.CommandType = System.Data.CommandType.StoredProcedure;
            cmd1.Parameters.AddWithValue("@usu", persona.usuario.correo);
            cmd1.Parameters.AddWithValue("@contra", persona.usuario.contrasena);
            cmd1.Parameters.AddWithValue("@fkPersona", persona.idPersona);


            cmd.ExecuteNonQuery();
            cmd1.ExecuteNonQuery();
            cone.Close();
            return RedirectToAction("AgendaCrearUsuario", "Agenda");
        }

        public List<Persona> consultarUsuarios()
        {
            List<Persona> historialServicios = new List<Persona>();
            try
            {
                MySqlConnection cone = new(_contexto.Conexion);
                cone.Open();
                MySqlCommand cmd = new("PDFUsuariosActivos", cone);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Persona mas = new Persona();
                    mas.usu = reader.GetString("nombre_usuario");
                    mas.contra = reader.GetString("contra_usuario");
                    mas.nombre = reader.GetString("nombre_persona");
                    mas.apellido = reader.GetString("apellido_persona");
                    mas.telefono = reader.GetString("Telefono_Persona");
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


      