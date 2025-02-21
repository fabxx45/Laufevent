using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Laufevent.Controllers
{
    [Route("create-user-that-has-no-educard-and-no-class")]
    [ApiController]
    public class CreateUserFirstLastOrgController : ControllerBase
    {
        /// <summary>
        /// Inserts a new user without an educard number and school class.
        /// </summary>
        /// <param name="userInfo">User information (first name, last name, organization, etc.)</param>
        /// <returns>Returns the newly created user ID along with a success message.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a user without an educard and school class", 
                          Description = "Inserts user data (first name, last name, organization) into the database without an educard number and school class.")]
        [SwaggerResponse(200, "Data inserted successfully.", typeof(object))]
        [SwaggerResponse(400, "Bad Request - Invalid data provided.")]
        [SwaggerResponse(500, "Internal Server Error.")]
        public async Task<IActionResult> InsertUserInformation([FromBody] CreateUserVariablesFirstLastOrg userInfo)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString.connectionstring))
                {
                    await connection.OpenAsync();
                    var query = @"
                        INSERT INTO Userinformation (firstname, lastname, uid, school_class, organisation, early_starter) 
                        VALUES (@firstname, @lastname, @uid, @school_class, @organisation, @early_starter)
                        RETURNING id;";  // PostgreSQL uses RETURNING instead of SCOPE_IDENTITY()

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@firstname", userInfo.firstname);
                        command.Parameters.AddWithValue("@lastname", userInfo.lastname);
                        command.Parameters.AddWithValue("@uid", DBNull.Value);  // No educard number
                        command.Parameters.AddWithValue("@school_class", DBNull.Value);    // No school class
                        command.Parameters.AddWithValue("@early_starter", DBNull.Value);   // No early starter info
                        command.Parameters.AddWithValue("@organisation", userInfo.organisation);

                        var newId = await command.ExecuteScalarAsync();  // Fetch the newly inserted ID
                        return Ok(new { Id = newId, Message = "Data inserted successfully." });
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
