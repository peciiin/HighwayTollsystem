using HighwayTollsystem.DTOs;
using HighwayTollsystem.Interfaces;
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
        private readonly ITollService _tollService;
        private readonly HighwayTollContext _db;

        public TollController(ITollService tollService, HighwayTollContext db)
        {
            _tollService = tollService;
            _db = db;
        }

        // POST: api/toll/passage
        [HttpPost]
        public async Task<ActionResult<PassageResponseDto>> CreatePassage([FromBody] RegisterTollPassDto dto)
        {
            var result = await _tollService.PassageProcessingAsync(dto);
            if (result == null) return NotFound($"Toll gate {dto.TollGateId} not found.");

            return Ok(result);
        }
    }

}