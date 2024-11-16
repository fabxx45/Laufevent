using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace Laufevent.Controllers;

[Route("create user that has no educard and no class")]
[ApiController]
public class CreateUserVorNachOrgController : ControllerBase
{
    private readonly string Bestzeit = "00:00:00";
    private int? Educardnr = null;
    private bool Fruehstarter = false;
    private string Klasse = null;
    private readonly int Runden = 0;

    [HttpPost]
    public IActionResult InsertUserInformation([FromBody] CreateUserVariablesVorNachOrg userInfo)
    {
        try
        {
            using (var connection = new SqlConnection(ConnectionString.connectionstring))
            {
                connection.Open();
                var query =
                    "INSERT INTO Userinformation (Vorname, Nachname, Educardnr, Klasse, Organisation, Runden, Bestzeit, Fruehstarter) " +
                    "VALUES (@Vorname, @Nachname, @Educardnr, @Klasse, @Organisation, @Runden, @Bestzeit, @Fruehstarter)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Vorname", userInfo.Vorname);
                    command.Parameters.AddWithValue("@Nachname", userInfo.Nachname);
                    command.Parameters.AddWithValue("@Runden", Runden);
                    command.Parameters.AddWithValue("@Bestzeit", Bestzeit);
                    command.Parameters.AddWithValue("@Educardnr", DBNull.Value);
                    command.Parameters.AddWithValue("@Klasse", DBNull.Value);
                    command.Parameters.AddWithValue("@Fruehstarter", DBNull.Value);
                    command.Parameters.AddWithValue("@Organisation", userInfo.Organisation); 
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