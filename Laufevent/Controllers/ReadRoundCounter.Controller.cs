using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Laufevent.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReadRoundCounterController : ControllerBase
{
    [HttpGet]
    public IActionResult GetRoundsById(int id)
    {
        try
        {
            using (var connection = new SqlConnection(ConnectionString.connectionstring))
            {
                connection.Open();
                var query = "SELECT * FROM Rounds WHERE id = @id";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var Rounds = new
                            {
                                Id = reader["id"],
                                Scantime = reader["Scantime"],
                            };
                            return Ok(Rounds);
                        }
                        else
                        {
                            return NotFound($"User with ID {id} not found.");
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