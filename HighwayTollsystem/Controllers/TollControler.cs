using HighwayTollsystem.Models;
using HighwayTollsystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace HighwayTollsystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TollController : ControllerBase
    {
        private readonly TollService _tollService;

        public TollController(TollService tollService)
        {
            _tollService = tollService;
        }

        // POST: api/toll/passage
        [HttpPost("passage")]
        public async Task<IActionResult> SimulatePassage([FromBody] PassageSimulateDto dto)
        {
            var passage = new Passage
            {
                Spz = dto.Spz.ToUpper(),
                GateId = dto.GateId,
                Timestamp = dto.Timestamp ?? DateTime.Now,
                VehicleSpeed = dto.VehicleSpeed
            };

            try
            {
                await _tollService.PassageProcessingAsync(passage);

                return Ok(new
                {
                    message = "Passage successfully processed and checked by the toll system.",
                    analyzedPassage = passage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing the toll: {ex.Message}");
            }
        }
    }

    public class PassageSimulateDto
    {
        public string Spz { get; set; } = null!;
        public int GateId { get; set; }
        public int VehicleSpeed { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}