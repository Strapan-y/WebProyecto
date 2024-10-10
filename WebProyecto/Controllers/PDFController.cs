using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using WebProyecto.Data;

namespace WebProyecto.Controllers
{
    public class PDFController : Controller
    {

        private readonly Contexto _contexto;

        public PDFController(Contexto contexto)
        {
            _contexto = contexto;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GenerarExcelAgendas()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Agendas");

                // Encabezados
                worksheet.Cell(1, 1).Value = "Fecha";
                worksheet.Cell(1, 2).Value = "Hora";
                worksheet.Cell(1, 3).Value = "Servicio";
                worksheet.Cell(1, 4).Value = "Persona";
                worksheet.Cell(1, 5).Value = "Descripcion";

                MySqlConnection cone = new(_contexto.Conexion);
                cone.Open();
                MySqlCommand cmd = new("consultarHistorialAgendas", cone);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader reader = cmd.ExecuteReader();

                int currentRow = 2;
                while (reader.Read())
                {
                    worksheet.Cell(currentRow, 1).Value = reader.GetString("fechainicio_age").Substring(0, 10);
                    worksheet.Cell(currentRow, 2).Value = reader.GetString("horainicio_age");
                    worksheet.Cell(currentRow, 3).Value = reader.GetString("Nombre_Servicio");
                    worksheet.Cell(currentRow, 4).Value = reader.GetString("nombre_persona");
                    worksheet.Cell(currentRow, 5).Value = reader.GetString("Descripcion_Servicio");

                    currentRow++;
                }
                cone.Close();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte__Historial_Agendas.xlsx");
                }
            }
        }

        public FileResult GenerarPdfAgendas()
        {
            // Crear un nuevo documento
            Document doc = new Document();

            byte[] buffer;


            using (MemoryStream ms = new MemoryStream())
            {

                PdfWriter.GetInstance(doc, ms);
                doc.Open();
                Paragraph title = new Paragraph("Reporte Historial Agendas");
                title.Alignment = Element.ALIGN_CENTER;
                doc.Add(title);

                Paragraph espacio = new Paragraph(" ");
                doc.Add(espacio);

                MySqlConnection cone = new(_contexto.Conexion);
                cone.Open();
                MySqlCommand cmd = new("consultarHistorialAgendas", cone);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader reader = cmd.ExecuteReader();


                List<objectoadop> listadoadopciones = new List<objectoadop>();

                while (reader.Read())
                {

                    listadoadopciones.Add(new objectoadop
                    {
                        item0 = reader.GetString("fechainicio_age").Substring(0, 10),
                        item1 = reader.GetString("horainicio_age"),
                        item2 = reader.GetString("Nombre_Servicio"),
                        item3 = reader.GetString("nombre_persona"),
                        item4 = reader.GetString("Descripcion_Servicio")



                });
                }
                cone.Close();


                PdfPTable table = new PdfPTable(5);

                List<string> titulos = new List<string>()
                {
                    "Fecha","Hora","Servicio","Persona","Descripcion"
                };



                for (int i = 0; i < 5; i++)
                {
                    PdfPCell celda1 = new PdfPCell(new Phrase(titulos[i]));
                    celda1.BackgroundColor = new BaseColor(130, 130, 130);
                    celda1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    table.AddCell(celda1);
                }

                for (int i = 0; i < listadoadopciones.Count; i++)
                {
                    table.AddCell(listadoadopciones[i].item0);
                    table.AddCell(listadoadopciones[i].item1);
                    table.AddCell(listadoadopciones[i].item2);
                    table.AddCell(listadoadopciones[i].item3);
                    table.AddCell(listadoadopciones[i].item4);
                }

                doc.Add(table);
                doc.Close();
                buffer = ms.ToArray();
            }

            return File(buffer, "application/pdf");
        }

        public FileResult GenerarPdfUusuarios()
        {
            // Crear un nuevo documento
            Document doc = new Document();

            byte[] buffer;


            using (MemoryStream ms = new MemoryStream())
            {

                PdfWriter.GetInstance(doc, ms);
                doc.Open();
                Paragraph title = new Paragraph("Reporte Solicitudes");
                title.Alignment = Element.ALIGN_CENTER;
                doc.Add(title);

                Paragraph espacio = new Paragraph(" ");
                doc.Add(espacio);

                MySqlConnection cone = new(_contexto.Conexion);
                cone.Open();
                MySqlCommand cmd = new("PDFUsuariosActivos", cone);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader reader = cmd.ExecuteReader();


                List<objectoadop> listadoadopciones = new List<objectoadop>();

                while (reader.Read())
                {

                    listadoadopciones.Add(new objectoadop
                    {
                        item0 = reader.GetString("nombre_usuario"),
                        item1 = reader.GetString("contra_usuario"),
                        item2 = reader.GetString("nombre_persona"),
                        item3 = reader.GetString("apellido_persona"),
                        item4 = reader.GetString("Telefono_Persona")
                    });
                }
                cone.Close();


                PdfPTable table = new PdfPTable(5);

                List<string> titulos = new List<string>()
                {
                    "Usuario","Contraseña","Nombre Usuario","Apellido","Telefono"
                };



                for (int i = 0; i < 5; i++)
                {
                    PdfPCell celda1 = new PdfPCell(new Phrase(titulos[i]));
                    celda1.BackgroundColor = new BaseColor(130, 130, 130);
                    celda1.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    table.AddCell(celda1);
                }

                for (int i = 0; i < listadoadopciones.Count; i++)
                {
                    table.AddCell(listadoadopciones[i].item0);
                    table.AddCell(listadoadopciones[i].item1);
                    table.AddCell(listadoadopciones[i].item2);
                    table.AddCell(listadoadopciones[i].item3);
                    table.AddCell(listadoadopciones[i].item4);
                }

                doc.Add(table);
                doc.Close();
                buffer = ms.ToArray();
            }

            return File(buffer, "application/pdf");
        }


        public IActionResult GenerarExcelUsuarios()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Usuarios");

                // Encabezados
                worksheet.Cell(1, 1).Value = "Usuario";
                worksheet.Cell(1, 2).Value = "Contraseña";
                worksheet.Cell(1, 3).Value = "Nombre";
                worksheet.Cell(1, 4).Value = "Apellido";
                worksheet.Cell(1, 5).Value = "Telefono";


                MySqlConnection cone = new(_contexto.Conexion);
                cone.Open();
                MySqlCommand cmd = new("PDFUsuariosActivos", cone);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                MySqlDataReader reader = cmd.ExecuteReader();

                int currentRow = 2;
                while (reader.Read())
                {
                    worksheet.Cell(currentRow, 1).Value = reader.GetString("nombre_usuario");
                    worksheet.Cell(currentRow, 2).Value = reader.GetString("contra_usuario");
                    worksheet.Cell(currentRow, 3).Value = reader.GetString("nombre_persona");
                    worksheet.Cell(currentRow, 4).Value = reader.GetString("apellido_persona");
                    worksheet.Cell(currentRow, 5).Value = reader.GetString("Telefono_Persona");


                    currentRow++;
                }
                cone.Close();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte__Historial_Usuarios.xlsx");
                }
            }
        }

    }
}

public class objectoadop
{
    public string item0 { get; set; } = null!;
    public string item1 { get; set; } = null!;
    public string item2 { get; set; } = null!;
    public string item3 { get; set; } = null!;
    public string item4 { get; set; } = null!;
    public string item5 { get; set; } = null!;
    public string item6 { get; set; } = null!;

}
