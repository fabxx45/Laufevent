using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data.SqlClient;

namespace Laufevent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadRoundCounterController : ControllerBase
    {
        /// <summary>
        /// Retrieves the count of rounds for a given user ID.
        /// </summary>
        /// <param name="id">The ID of the user whose rounds count you want to retrieve.</param>
        /// <returns>Returns the count of rounds for the specified user ID or an error message if no rounds exist.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get the rounds count for a given user ID", 
                          Description = "Fetches the total number of rounds for a specific user ID.")]
        [SwaggerResponse(200, "Rounds count successfully retrieved.", typeof(object))]
        [SwaggerResponse(404, "No rounds found for the given user ID.")]
        [SwaggerResponse(500, "Internal Server Error - Database issue or unexpected error.")]
        public IActionResult GetRoundsCountById(int id)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString.connectionstring))
                {
                    connection.Open();
                    var query = "SELECT COUNT(*) AS RoundsCount FROM Rounds WHERE id = @id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var result = new
                                {
                                    RoundsCount = reader["RoundsCount"]
                                };
                                return Ok(result);
                            }
                            else
                            {
                                return NotFound($"No entries found for ID {id}.");
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
}
