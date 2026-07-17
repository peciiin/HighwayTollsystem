using HighwayTollsystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HighwayTollsystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly HighwayTollContext _db;

        public VehicleController(HighwayTollContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vehicles = await _db.Vehicles.Include(v => v.Type).ToListAsync();
            return Ok(vehicles);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] VehicleDto dto)
        {
            var typeExists = await _db.VehicleTypes.AnyAsync(t => t.Id == dto.VehicleTypeId);
            if (!typeExists)
            {
                return BadRequest("The specified VehicleTypeId does not exist in the database!");
            }

            var exists = await _db.Vehicles.AnyAsync(v => v.Spz == dto.Spz);
            if (exists)
            {
                return BadRequest($"Vehicle with SPZ '{dto.Spz}' is already registered.");
            }

            var vehicle = new Vehicle
            {
                Spz = dto.Spz.ToUpper(),
                TypeId = dto.VehicleTypeId,
                EmissionClass = dto.EmissionClass ?? "EURO 6",
                RegisteredAt = DateTime.Now
            };

            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync();

            return Ok(new { message = $"Vehicle {vehicle.Spz} successfully registered.", vehicle });
        }
    }

    public class VehicleDto
    {
        public string Spz { get; set; } = null!;
        public int VehicleTypeId { get; set; }
        public string? EmissionClass { get; set; }
    }
}