using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data.SqlClient;

namespace Laufevent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateUserController : ControllerBase
    {
        /// <summary>
        /// Updates user information for a specific user identified by ID.
        /// </summary>
        /// <param name="id">The ID of the user to be updated.</param>
        /// <param name="userInfo">An object containing the updated user details.</param>
        /// <returns>Returns a message indicating the result of the update operation.</returns>
        [HttpPut]
        [SwaggerOperation(Summary = "Update user information by ID", 
                          Description = "Updates the user details such as first name, last name, education card number, school class, and organization.")]
        [SwaggerResponse(200, "User details successfully updated.", typeof(string))]
        [SwaggerResponse(404, "User with the specified ID not found.")]
        [SwaggerResponse(500, "Internal Server Error - Database issue or unexpected error.")]
        public IActionResult UpdateUserById(int id, [FromBody] UpdateUserModel userInfo)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString.connectionstring))
                {
                    connection.Open();

                    var query = @"UPDATE Userinformation SET firstname = @firstName, lastname = @lastName, educard_number = @eduCardNumber, school_class = @schoolClass, organisation = @organisation WHERE id = @id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@firstName", userInfo.FirstName);
                        command.Parameters.AddWithValue("@lastName", userInfo.LastName);
                        command.Parameters.AddWithValue("@eduCardNumber", (object)userInfo.EduCardNumber ?? DBNull.Value);
                        command.Parameters.AddWithValue("@schoolClass", (object)userInfo.SchoolClass ?? DBNull.Value);
                        command.Parameters.AddWithValue("@organisation", (object)userInfo.Organisation ?? DBNull.Value);

                        var rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok($"User with ID {id} successfully updated.");
                        }
                        else
                        {
                            return NotFound($"User with ID {id} not found.");
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
