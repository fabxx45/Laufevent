using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data.SqlClient;

namespace Laufevent.Controllers
{
    [Route("create-user-that-has-no-educard-and-no-class")]
    [ApiController]
    public class CreateUserFirstLastOrgController : ControllerBase
    {
        private int? educard_number = null;
        private bool early_starter = false;
        private string school_class = null;

        /// <summary>
        /// Inserts a new user without an educard number and class.
        /// </summary>
        /// <param name="userInfo">User information (first name, last name, organization, etc.)</param>
        /// <returns>Returns the newly created user ID along with a success message.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a user without an educard and class", 
                          Description = "Inserts user data (first name, last name, organization) into the database without an educard number and school class.")]
        [SwaggerResponse(200, "Data inserted successfully.", typeof(object))]
        [SwaggerResponse(400, "Bad Request - Invalid data provided.")]
        [SwaggerResponse(500, "Internal Server Error.")]
        public IActionResult InsertUserInformation([FromBody] CreateUserVariablesFirstLastOrg userInfo)
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
                        command.Parameters.AddWithValue("@school_class", DBNull.Value);    // No school class
                        command.Parameters.AddWithValue("@early_starter", DBNull.Value);   // No early starter info
                        command.Parameters.AddWithValue("@organisation", userInfo.organisation);

                        var newId = command.ExecuteScalar();  // Fetch the newly inserted ID
                        return Ok(new { Id = newId, Message = "Data inserted successfully." });
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
