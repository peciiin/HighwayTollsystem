using HighwayTollsystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HighwayTollsystem.Enums;
using HighwayTollsystem.DTOs;
using HighwayTollsystem.Interfaces;
using FluentValidation;
namespace HighwayTollsystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleResponseDto>>> GetVehicles([FromQuery] VehicleFilterDto filter)
        {
            var vehicles = await _vehicleService.GetVehiclesAsync(filter);
            return Ok(vehicles);
        }


        [HttpGet("{id:long}")]
        public async Task<ActionResult<VehicleResponseDto>> GetVehicleById(long id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Vehicle Not Found",
                    Detail = $"Vehicle with ID {id} was not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(vehicle);
        }



        [HttpPost]
        public async Task<ActionResult<VehicleResponseDto>> CreateVehicle([FromBody] RegisterNewVehicleDto dtoVehicle)
        {
            var result = await _vehicleService.CreateVehicleAsync(dtoVehicle);
            if (result is null)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Duplicate SPZ",
                    Detail = $"Vehicle with SPZ '{dtoVehicle.Spz.Trim().ToUpper()}' is already registered.",
                    Status = StatusCodes.Status409Conflict
                });
            }

            return CreatedAtAction(nameof(GetVehicleById), new { id = result.VehicleId }, result);
        }
    }

}