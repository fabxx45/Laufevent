using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Laufevent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompleteRoundController : ControllerBase
    {
        [HttpPost]
        [SwaggerOperation(Summary = "Inserts user information into the database", 
            Description = "This endpoint inserts the user's UID and scan time into the Rounds table.")]
        [SwaggerResponse(200, "Data inserted successfully.", typeof(string))]
        [SwaggerResponse(500, "Internal Server Error.")]
        public async Task<IActionResult> InsertUserInformation([FromBody] CompleteRoundVariables userInfo)
        {
            try
            {
                DateTime currentTime = DateTime.Now;

                using (var connection = new SqlConnection(ConnectionString.connectionstring))
                {
                    await connection.OpenAsync();
                    var query = "INSERT INTO Rounds (UID, Scantime) VALUES (@UID, @Scantime);";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@UID", SqlDbType.Float).Value = (float)userInfo.UID;

                        command.Parameters.Add("@Scantime", SqlDbType.DateTime).Value = currentTime;

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return Ok($"Data inserted successfully. Rows affected: {rowsAffected}");
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