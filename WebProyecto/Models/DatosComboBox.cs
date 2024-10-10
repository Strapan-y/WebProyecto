using MySql.Data.MySqlClient;

namespace WebProyecto.Models

{
    public class DatosComboBox
    {

        string connectionString = "Server=localhost;Port=3306;Database=agenda;Uid=root;Pwd=root;";

        public List<Persona> ObtenerDatos()
        {
            List<Persona> datos = new List<Persona>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT persona.nombre_persona, usuario.nombre_usuario \r\nFROM agenda.usuario \r\ninner join agenda.persona on usuario.FK_persona=persona.numdocu_persona where FK_ID_Rol=2;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Persona persona = new Persona
                            {
                                nombre = reader["nombre_persona"].ToString(),
                                correo = reader["nombre_usuario"].ToString()
                            };
                            datos.Add(persona);
                        }
                    }
                }
            }

            return datos;
        }

        public List<Persona> ObtenerDatosTipoDocumento()
        {
            List<Persona> datos = new List<Persona>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM agenda.tipodocumento where Estado_TipoDocumento=1;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Persona persona = new Persona
                            {
                                nombre = reader["ID_TipoDocumento"].ToString(),
                                correo = reader["Nombre_TipoDocumento"].ToString()
                            };
                            datos.Add(persona);
                        }
                    }
                }
            }

            return datos;
        }

        public List<Persona> ObtenerDatosLugar()
        {
            List<Persona> datos = new List<Persona>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT idLugar,nom_lugar FROM agenda.lugar;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Persona persona = new Persona
                            {
                                correo = reader["idLugar"].ToString(),
                                nombre = reader["nom_lugar"].ToString()
                            };
                            datos.Add(persona);
                        }
                    }
                }
            }

            return datos;
        }

        public List<ComboBox> ObtenerLugar()
        {
            List<ComboBox> datos = new List<ComboBox>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT idLugar,nom_lugar FROM agenda.lugar;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ComboBox persona = new ComboBox
                            {
                                id = reader["idLugar"].ToString(),
                                nombre = reader["nom_lugar"].ToString()
                            };
                            datos.Add(persona);
                        }
                    }
                }
            }

            return datos;
        }

        public List<ComboBox> ObtenerDatosTipoServicio()
        {
            List<ComboBox> datos = new List<ComboBox>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT ID_Servicio,Nombre_Servicio FROM agenda.servicio where Estado_Servicio=1;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ComboBox persona = new ComboBox
                            {
                                id = reader["ID_Servicio"].ToString(),
                                nombre = reader["Nombre_Servicio"].ToString()
                            };
                            datos.Add(persona);
                        }
                    }
                }
            }

            return datos;
        }

        public List<ComboBox> ObtenerDatosTiposDocumento()
        {
            List<ComboBox> datos = new List<ComboBox>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM agenda.tipodocumento where Estado_TipoDocumento=1;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ComboBox persona = new ComboBox
                            {
                                id = reader["ID_TipoDocumento"].ToString(),
                                nombre = reader["Nombre_TipoDocumento"].ToString()
                            };
                            datos.Add(persona);
                        }
                    }
                }
            }

            return datos;
        }

        public List<string> ObtenerDatosDocumento()
        {
            List<string> datos = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Nombre_TipoDocumento FROM agenda.tipodocumento;";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            string dato = reader["Nombre_TipoDocumento"].ToString();
                            datos.Add(dato);
                        }
                    }
                }
            }

            return datos;
        }
    }

}
