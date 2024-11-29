using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Laufevent.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReadUserByNameController : ControllerBase
{
    [HttpGet]
    public IActionResult GetUserByName([FromQuery] string firstName, [FromQuery] string lastName)
    {
        try
        {
            using (var connection = new SqlConnection(ConnectionString.connectionstring))
            {
                connection.Open();
                var query = "SELECT * FROM Userinformation WHERE firstname = @firstName AND lastname = @lastName";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@lastName", lastName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var user = new
                            {
                                Id = reader["id"],
                                FirstName = reader["firstname"],
                                LastName = reader["lastname"],
                                EduCardNumber = reader["educard_number"],
                                SchoolClass = reader["school_class"],
                                Organisation = reader["organisation"],
                                EarlyStarter = reader["early_starter"]
                            };
                            return Ok(user);
                        }
                        else
                        {
                            return NotFound($"User with FirstName '{firstName}' and LastName '{lastName}' not found.");
                        }
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
