using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace Laufevent.Controllers;

[Route("create user that has no educard and no class")]
[ApiController]
public class CreateUserVorNachOrgController : ControllerBase
{
    private readonly string fastest_lap = "00:00:00";
    private int? educard_number = null;
    private bool early_starter = false;
    private string school_class = null;
    private readonly int laps = 0;

    [HttpPost]
    public IActionResult InsertUserInformation([FromBody] CreateUserVariablesVorNachOrg userInfo)
    {
        try
        {
            using (var connection = new SqlConnection(ConnectionString.connectionstring))
            {
                connection.Open();
                var query =
                    "INSERT INTO Userinformation (firstname, lastname, educard_number, school_class, organisation, laps, fastest_lap, early_starter) " +
                    "VALUES (@firstname, @lastname, @educard_number, @school_class, @organisation, @laps, @fastest_lap, @early_starter)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@firstname", userInfo.firstname);
                    command.Parameters.AddWithValue("@lastname", userInfo.lastname);
                    command.Parameters.AddWithValue("@laps", laps);
                    command.Parameters.AddWithValue("@fastest_lap", fastest_lap);
                    command.Parameters.AddWithValue("@educard_number", DBNull.Value);
                    command.Parameters.AddWithValue("@school_class", DBNull.Value);
                    command.Parameters.AddWithValue("@early_starter", DBNull.Value);
                    command.Parameters.AddWithValue("organisation", userInfo.organisation); 
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
