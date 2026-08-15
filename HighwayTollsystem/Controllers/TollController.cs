using HighwayTollsystem.DTOs;
using HighwayTollsystem.Models;
using HighwayTollsystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HighwayTollsystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TollController : ControllerBase
    {
        private readonly TollService _tollService;
        private readonly HighwayTollContext _db;

        public TollController(TollService tollService, HighwayTollContext db)
        {
            _tollService = tollService;
            _db = db;
        }

        // POST: api/toll/passage
        [HttpPost("passage")]
        public async Task<IActionResult> RegisterTollPass([FromBody] RegisterTollPassDto passDto)
        {
            var gateExists = await _db.TollGates.AsNoTracking().AnyAsync(g => g.GateId == passDto.TollGateId);
            if (!gateExists) return BadRequest($"Toll gate with ID {passDto.TollGateId} does not exist.");
            

            var passage = new Passage
            {
                GateId = passDto.TollGateId,
                VehicleSpeed = passDto.VehicleSpeed,
                Timestamp = DateTime.UtcNow
            };


            await _tollService.PassageProcessingAsync(passage, passDto.DetectedSpz);

            return Ok(passage);
        }
    }

}