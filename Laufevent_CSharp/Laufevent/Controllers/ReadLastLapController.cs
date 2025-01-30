using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data.SqlClient;
using System.Collections.Generic;
using System;

namespace Laufevent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadLastLapController : ControllerBase
    {
        /// <summary>
        /// Retrieves the lap duration based on the last two scan times for the given ID.
        /// </summary>
        /// <param name="id">The ID of the user to fetch the lap times for.</param>
        /// <returns>Returns the lap duration (time difference between the last two laps) or an error message.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get the lap duration based on the last two scan times",
                          Description = "Fetches the last two scan times for the given ID and calculates the lap duration.")]
        [SwaggerResponse(200, "Lap duration successfully calculated.", typeof(object))]
        [SwaggerResponse(404, "Not enough data to calculate lap duration.")]
        [SwaggerResponse(500, "Internal Server Error - Database issue or unexpected error.")]
        public IActionResult GetLastLapById(int id)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString.connectionstring))
                {
                    connection.Open();

                    var query = @"SELECT TOP 2 Scantime FROM Rounds WHERE id = @id ORDER BY Scantime DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (var reader = command.ExecuteReader())
                        {
                            var times = new List<TimeSpan>();

                            while (reader.Read())
                            {
                                times.Add((TimeSpan)reader["Scantime"]);
                            }

                            if (times.Count < 2)
                            {
                                return NotFound($"Not enough data for ID {id} to calculate lap duration.");
                            }

                            var lapDuration = times[0] - times[1];

                            var result = new
                            {
                                Id = id,
                                LapDuration = lapDuration.ToString(@"hh\:mm\:ss\.fff") // Format TimeSpan as string
                            };

                            return Ok(result);
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
