using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Laufevent.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UpdateUserController : ControllerBase
{
    [HttpPut]
    public IActionResult UpdateUserById(int id, [FromBody] UpdateUserModel userInfo)
    {
        try
        {
            using (var connection = new SqlConnection(ConnectionString.connectionstring))
            {
                connection.Open();
                
                var query = @"UPDATE Userinformation SET firstname = @firstName, lastname = @lastName, educard_number = @eduCardNumber, school_class = @schoolClass, organisation = @organisation, early_starter = @earlyStarter WHERE id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@firstName", userInfo.FirstName);
                    command.Parameters.AddWithValue("@lastName", userInfo.LastName);
                    command.Parameters.AddWithValue("@eduCardNumber", (object)userInfo.EduCardNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@schoolClass", (object)userInfo.SchoolClass ?? DBNull.Value);
                    command.Parameters.AddWithValue("@organisation", (object)userInfo.Organisation ?? DBNull.Value);
                    command.Parameters.AddWithValue("@earlyStarter", (object)userInfo.EarlyStarter ?? DBNull.Value);

                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok($"User with ID {id} successfully updated.");
                    }
                    else
                    {
                        return NotFound($"User with ID {id} not found.");
                    }
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

