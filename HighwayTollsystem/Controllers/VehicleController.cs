using HighwayTollsystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HighwayTollsystem.Enums;
using HighwayTollsystem.DTOs;
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
        public async Task<ActionResult> GetVehicles([FromQuery] VehicleFilterDto filter)
        {
            var query = _db.Vehicles.AsNoTracking().AsQueryable();
            if (filter.FuelType.HasValue)
            {
                query = query.Where(v => v.FuelType == filter.FuelType.Value);
            }
            if (filter.VehicleType.HasValue)
            {
                query = query.Where(v => v.Type == filter.VehicleType.Value);
            }
            if (!string.IsNullOrWhiteSpace(filter.Spz))
            {
                var spz = filter.Spz.ToUpper();
                query = query.Where(v => v.Spz.Contains(spz));
            }
            if (filter.RegisteredFrom.HasValue)
            {
                query = query.Where(v => v.RegisteredAt >= filter.RegisteredFrom.Value);
            }
            if (filter.RegisteredTo.HasValue)
            {
                query = query.Where(v => v.RegisteredAt <= filter.RegisteredTo.Value);
            }
            if (!string.IsNullOrEmpty(filter.CountryCode))
            {
                query = query.Where(v => v.CountryCode == filter.CountryCode);
            }
            if (!string.IsNullOrWhiteSpace(filter.Vin))
            {
                var vinUpper = filter.Vin.ToUpper();
                query = query.Where(v => v.Vin != null && v.Vin.Contains(vinUpper));
            }
            if (filter.EmissionClass.HasValue)
            {
                query = query.Where(v => v.EmissionClass == filter.EmissionClass.Value);
            }



            int skip = (filter.PageNumber - 1) * filter.PageSize;
            var vehicles = await query
                .OrderByDescending(v => v.RegisteredAt).Skip(skip).Take(filter.PageSize).ToListAsync();
            return Ok(vehicles);
        }


        [HttpPost]
        public async Task<ActionResult> CreateVehicle([FromBody] RegisterNewVehicle dtoVehicle)
        {
            var countryCode = dtoVehicle.CountryCode?.ToUpper() ?? "CZ";
            if (string.IsNullOrWhiteSpace(dtoVehicle.Spz)) return BadRequest("No SPZ entered.");
            
            var spz = dtoVehicle.Spz.ToUpper();
            var exist = await _db.Vehicles.AnyAsync(v => v.Spz == spz);
            if (exist)
            {
                return BadRequest($"Vehicle with SPZ {spz} already exists.");
            }
            var vehicle = new Vehicle
            {
                Spz = spz,
                Type = dtoVehicle.Type,
                FuelType = dtoVehicle.FuelType,
                EmissionClass = dtoVehicle.EmissionClass,
                CountryCode = countryCode,
                Vin = dtoVehicle.Vin?.ToUpper(),
                RegisteredAt = DateTime.UtcNow
            };

            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVehicles), new { spz = vehicle.Spz }, vehicle);
        }
    }

}