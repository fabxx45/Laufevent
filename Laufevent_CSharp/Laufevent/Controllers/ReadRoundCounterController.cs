using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

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
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get the rounds count for a given user ID", 
                          Description = "Fetches the total number of rounds for a specific user ID.")]
        [SwaggerResponse(200, "Rounds count successfully retrieved.", typeof(object))]
        [SwaggerResponse(404, "No rounds found for the given user ID.")]
        [SwaggerResponse(500, "Internal Server Error - Database issue or unexpected error.")]
        public async Task<IActionResult> GetRoundsCountById(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString.connectionstring))
                {
                    await connection.OpenAsync();
                    var query = "SELECT COUNT(*) AS RoundsCount FROM Rounds WHERE id = @id";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        var roundsCount = await command.ExecuteScalarAsync();

                        if (roundsCount != null && Convert.ToInt32(roundsCount) > 0)
                        {
                            return Ok(new { RoundsCount = roundsCount });
                        }
                        else
                        {
                            return NotFound($"No entries found for ID {id}.");
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
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
