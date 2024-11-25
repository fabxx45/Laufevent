using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Laufevent.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompleteRound : ControllerBase
{
    [HttpPost]
    public IActionResult InsertUserInformation([FromBody] CompleteRoundVariables userInfo)
    {
        try
        {
            string formattedTime = DateTime.Now.ToString("HH:mm:ss");

            using (var connection = new SqlConnection(ConnectionString.connectionstring))
            {
                connection.Open();
                var query = "INSERT INTO Rounds (ID, Scantime) VALUES (@ID, @Scantime);";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@ID", SqlDbType.Int).Value = userInfo.ID;
                    command.Parameters.Add("@Scantime", SqlDbType.VarChar).Value = formattedTime;

                    var rowsAffected = command.ExecuteNonQuery();
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