using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace Laufevent.Controllers;

[Route("create user that has no educard")]
[ApiController]
public class CreateUserFirstLastOrgClassController : ControllerBase
{
    private int? educard_number = null;
    private bool early_starter = false;

    [HttpPost]
    public IActionResult InsertUserInformation([FromBody] CreateUserVariablesFirstLastOrgClass userInfo)
    {
        try
        {
            using (var connection = new SqlConnection(ConnectionString.connectionstring))
            {
                connection.Open();
                var query =
                    "INSERT INTO Userinformation (firstname, lastname, educard_number, school_class, organisation,  early_starter) " +
                    "VALUES (@firstname, @lastname, @educard_number, @school_class, @organisation,  @early_starter)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@firstname", userInfo.firstname);
                    command.Parameters.AddWithValue("@lastname", userInfo.lastname);
                    command.Parameters.AddWithValue("@educard_number", DBNull.Value);
                    command.Parameters.AddWithValue("@school_class", userInfo.school_class);
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