using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data.SqlClient;

namespace Laufevent.Controllers
{
    [Route("create-user-that-has-no-educard")]
    [ApiController]
    public class CreateUserFirstLastOrgClassController : ControllerBase
    {
        private int? educard_number = null;
        private bool early_starter = false;

        /// <summary>
        /// Inserts a new user who does not have an educard.
        /// </summary>
        /// <param name="userInfo">User information (first name, last name, etc.)</param>
        /// <returns>Returns the newly created user ID along with a success message.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a user without an educard number", 
                          Description = "Inserts user data (first name, last name, school class, organization) into the database for users without an educard number.")]
        [SwaggerResponse(200, "Data inserted successfully.", typeof(object))]
        [SwaggerResponse(400, "Bad Request - Invalid data provided.")]
        [SwaggerResponse(500, "Internal Server Error.")]
        public IActionResult InsertUserInformation([FromBody] CreateUserVariablesFirstLastOrgClass userInfo)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString.connectionstring))
                {
                    connection.Open();
                    var query = @"
                        INSERT INTO Userinformation (firstname, lastname, educard_number, school_class, organisation, early_starter) 
                        VALUES (@firstname, @lastname, @educard_number, @school_class, @organisation, @early_starter);
                        SELECT SCOPE_IDENTITY();"; 
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@firstname", userInfo.firstname);
                        command.Parameters.AddWithValue("@lastname", userInfo.lastname);
                        command.Parameters.AddWithValue("@educard_number", DBNull.Value);  // No educard number
                        command.Parameters.AddWithValue("@school_class", userInfo.school_class);
                        command.Parameters.AddWithValue("@early_starter", DBNull.Value);   // No early starter info
                        command.Parameters.AddWithValue("@organisation", userInfo.organisation);

                        var newUserId = command.ExecuteScalar();  // Fetch the newly inserted ID
                        
                        // Return the ID along with a success message
                        return Ok(new { Id = newUserId, Message = "Data inserted successfully." });
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
