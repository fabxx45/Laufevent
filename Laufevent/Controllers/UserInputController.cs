using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Laufevent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInformationController : ControllerBase
    {
        private readonly string _connectionString = @"Server=DESKTOP-N214O1C\SQLLAUFEVENT;Database=Laufevent;Trusted_Connection=True;";
        [HttpPost]
        public IActionResult InsertUserInformation([FromBody] UserInformation userInfo)
        {
            if (userInfo == null)
            {
                return BadRequest("User information is null");
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Userinformation (Vorname, Nachname, Runden, Bestzeit) " +
                                   "VALUES (@Vorname, @Nachname, @Runden, @Bestzeit)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Vorname", userInfo.Vorname);
                        command.Parameters.AddWithValue("@Nachname", userInfo.Nachname);
                        command.Parameters.AddWithValue("@Runden", userInfo.Runden);
                        command.Parameters.AddWithValue("@Bestzeit", userInfo.Bestzeit);

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
