using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data.SqlClient;

namespace Laufevent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadUserEduController : ControllerBase
    {
        /// <summary>
        /// Retrieves user information based on the provided EduCard number.
        /// </summary>
        /// <param name="educardNumber">The EduCard number of the user.</param>
        /// <returns>Returns the user details if found, otherwise a 404 not found error.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get user details by EduCard number", 
                          Description = "Fetches the complete user information for the given EduCard number.")]
        [SwaggerResponse(200, "User details retrieved successfully.", typeof(object))]
        [SwaggerResponse(404, "User with the specified EduCard number not found.")]
        [SwaggerResponse(500, "Internal Server Error - Database issue or unexpected error.")]
        public IActionResult GetUserByEduCardNumber(int educardNumber)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString.connectionstring))
                {
                    connection.Open();
                    const string query = "SELECT * FROM Userinformation WHERE educard_number = @educardNumber";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@educardNumber", educardNumber);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var user = new
                                {
                                    Id = reader["id"] is DBNull ? 0 : Convert.ToInt32(reader["id"]),
                                    FirstName = reader["firstname"]?.ToString(),
                                    LastName = reader["lastname"]?.ToString(),
                                    EduCardNumber = reader["educard_number"] is DBNull ? 0 : Convert.ToInt32(reader["educard_number"]),
                                    SchoolClass = reader["school_class"]?.ToString(),
                                    Organisation = reader["organisation"]?.ToString(),
                                    FastestLap = reader["fastest_lap"] is DBNull ? (int?)null : Convert.ToInt32(reader["fastest_lap"]),
                                    EarlyStarter = reader["early_starter"] is DBNull ? (bool?)null : Convert.ToBoolean(reader["early_starter"])
                                };
                                return Ok(user);
                            }
                            return NotFound($"User with EduCard number {educardNumber} not found.");
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
