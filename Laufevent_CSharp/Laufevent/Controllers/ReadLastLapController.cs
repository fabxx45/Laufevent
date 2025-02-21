﻿using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get the lap duration based on the last two scan times",
                          Description = "Fetches the last two scan times for the given ID and calculates the lap duration.")]
        [SwaggerResponse(200, "Lap duration successfully calculated.", typeof(object))]
        [SwaggerResponse(404, "Not enough data to calculate lap duration.")]
        [SwaggerResponse(500, "Internal Server Error - Database issue or unexpected error.")]
        public async Task<IActionResult> GetLastLapById(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString.connectionstring))
                {
                    await connection.OpenAsync();

                    var query = @"SELECT Scantime FROM Rounds WHERE id = @id ORDER BY Scantime DESC LIMIT 2";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var times = new List<DateTime>();

                            while (await reader.ReadAsync())
                            {
                                times.Add(reader.GetDateTime(0));
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
