using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data.SqlClient;

namespace Laufevent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadUserByNameController : ControllerBase
    {
        /// <summary>
        /// Retrieves user information based on the provided first and last name.
        /// </summary>
        /// <param name="firstName">The user's first name.</param>
        /// <param name="lastName">The user's last name.</param>
        /// <returns>Returns the user details if found, otherwise a 404 not found error.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get user details by first and last name", 
                          Description = "Fetches the complete user information based on the first and last name.")]
        [SwaggerResponse(200, "User details retrieved successfully.", typeof(object))]
        [SwaggerResponse(404, "User with the specified first and last name not found.")]
        [SwaggerResponse(500, "Internal Server Error - Database issue or unexpected error.")]
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
                                    FastestLap = reader["fastest_lap"],
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
}
