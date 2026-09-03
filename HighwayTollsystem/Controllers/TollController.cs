using HighwayTollsystem.DTOs;
using HighwayTollsystem.Interfaces;
using Microsoft.AspNetCore.Mvc;



namespace HighwayTollsystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TollController : ControllerBase
    {
        private readonly ITollService _tollService;

        public TollController(ITollService tollService)
        {
            _tollService = tollService;
        }

        [HttpPost]
        public async Task<ActionResult<PassageResponseDto>> RegisterTollPass([FromBody] RegisterTollPassDto dto)
        {
            var result = await _tollService.PassageProcessingAsync(dto);
            if (result == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Toll Gate Not Found",
                    Detail = $"No toll gate found with ID {dto.TollGateId}.",
                    Status = 404
                });
            }

            return Ok(result);
        }
    }

}