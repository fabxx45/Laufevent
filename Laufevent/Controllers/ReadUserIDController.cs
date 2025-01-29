using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data.SqlClient;

namespace Laufevent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadUserIDController : ControllerBase
    {
        /// <summary>
        /// Retrieves user information based on the provided user ID.
        /// </summary>
        /// <param name="id">The unique ID of the user.</param>
        /// <returns>Returns the user details if found, otherwise a 404 not found error.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get user details by ID", 
                          Description = "Fetches the complete user information for the given user ID.")]
        [SwaggerResponse(200, "User details retrieved successfully.", typeof(object))]
        [SwaggerResponse(404, "User with the specified ID not found.")]
        [SwaggerResponse(500, "Internal Server Error - Database issue or unexpected error.")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString.connectionstring))
                {
                    connection.Open();
                    var query = "SELECT * FROM Userinformation WHERE id = @id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

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
                                    FastestLap = reader["fastest_lap"],
                                    EarlyStarter = reader["early_starter"]
                                };
                                return Ok(user);
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
}
