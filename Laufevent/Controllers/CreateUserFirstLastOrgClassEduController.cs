using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data.SqlClient;

namespace Laufevent.Controllers
{
    [Route("create-user-that-has-everything")]
    [ApiController]
    public class CreateUserFirstLastOrgClassEduController : ControllerBase
    {
        private bool early_starter = false;

        /// <summary>
        /// Inserts a new user who has all required information (educard, school class, organisation, etc.)
        /// </summary>
        /// <param name="userInfo">User information (first name, last name, educard number, school class, organization, early starter)</param>
        /// <returns>Returns the newly created user ID along with a success message.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a user with all information", 
                          Description = "Inserts user data (first name, last name, educard number, school class, organization, early starter) into the database.")]
        [SwaggerResponse(200, "Data inserted successfully.", typeof(object))]
        [SwaggerResponse(400, "Bad Request - Invalid data provided.")]
        [SwaggerResponse(500, "Internal Server Error.")]
        public IActionResult InsertUserInformation([FromBody] CreateUserVariablesFirstLastOrgClassEdu userInfo)
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
                        command.Parameters.AddWithValue("@educard_number", userInfo.educard);
                        command.Parameters.AddWithValue("@school_class", userInfo.school_class);
                        command.Parameters.AddWithValue("@early_starter", DBNull.Value); // You can set a default if necessary
                        command.Parameters.AddWithValue("@organisation", userInfo.organisation);

                        var newUserId = command.ExecuteScalar();  // Fetch the newly inserted ID
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
