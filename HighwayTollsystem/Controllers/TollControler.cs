using Microsoft.AspNetCore.Mvc;
using HighwayTollsystem.Models;
using HighwayTollsystem.Services;

namespace HighwayTollsystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TollController : ControllerBase
    {
        public readonly ITollService _talService;
        public TollController(ITollService tollService) 
        {
            _talService = tollService;
        }

        [HttpPost("passage")]
        public async Task<IActionResult> RegisterPassage([FromBody] Passage passage)
        {
            if (passage == null)
            {
                return BadRequest("wrong data");

            }
            await _talService.PassageProcessingAsync(passage);
            return Ok(new { message = "Sucesfull." });
        }
    }
}
