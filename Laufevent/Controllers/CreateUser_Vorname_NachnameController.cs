using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Laufevent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateUser_Vorname_NachnameController : ControllerBase
    {
        private int Runden = 0;
        private string Bestzeit = "00:00:00";
        private int? Educardnr = null; 
        private string Klasse = null; 

        [HttpPost]
        public IActionResult InsertUserInformation([FromBody] CreateUservariables_Vorname_Nachname userInfo)
        {
            if (userInfo == null)
            {
                return BadRequest("User information is null");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString.Value))
                {
                    connection.Open();
                    string query =
                        "INSERT INTO Userinformation (Vorname, Nachname, Educardnr, Klasse, Organisation, Runden, Bestzeit) " +
                        "VALUES (@Vorname, @Nachname, @Educardnr, @Klasse, @Organisation, @Runden, @Bestzeit)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Vorname", userInfo.Vorname);
                        command.Parameters.AddWithValue("@Nachname", userInfo.Nachname);
                        command.Parameters.AddWithValue("@Runden", Runden);
                        command.Parameters.AddWithValue("@Bestzeit", Bestzeit);
                        command.Parameters.AddWithValue("@Educardnr", DBNull.Value);
                        command.Parameters.AddWithValue("@Klasse", DBNull.Value);

                        // Check for null or empty Organisation
                        if (!string.IsNullOrEmpty(userInfo.Organisation))
                        {
                            command.Parameters.AddWithValue("@Organisation", userInfo.Organisation);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@Organisation", DBNull.Value);
                        }

                        int rowsAffected = command.ExecuteNonQuery();
                        return Ok($"Data inserted successfully. Rows affected: {rowsAffected}");
                    }
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
