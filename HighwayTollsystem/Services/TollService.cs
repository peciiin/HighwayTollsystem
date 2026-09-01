using HighwayTollsystem.DTOs;
using HighwayTollsystem.Enums;
using HighwayTollsystem.Interfaces;
using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HighwayTollsystem.Services;

public class TollService : ITollService
{
    private readonly HighwayTollContext _db;
    private readonly IVignetteService _vignetteService;
    private readonly ISpeedService _speedService;
    private readonly IVehicleInspectionService _vehicleInspectionService;

    public TollService(
        HighwayTollContext db,
        IVignetteService vignetteService,
        ISpeedService speedService,
        IVehicleInspectionService vehicleInspectionService)
    {
        _db = db;
        _vignetteService = vignetteService;
        _speedService = speedService;
        _vehicleInspectionService = vehicleInspectionService;
    }

    public async Task<PassageResponseDto?> PassageProcessingAsync(RegisterTollPassDto registerPassDto)
    {
        var gateExists = await _db.TollGates.AsNoTracking().AnyAsync(g => g.GateId == registerPassDto.TollGateId);
        if (!gateExists)
        {
            return null;
        }

        var passage = new Passage
        {
            GateId = registerPassDto.TollGateId,
            VehicleSpeed = registerPassDto.VehicleSpeed,
            Timestamp = DateTime.UtcNow
        };
        var normalizedSpz = registerPassDto.DetectedSpz?.Trim().ToUpper() ?? string.Empty;
        var vehicle = await _db.Vehicles.AsNoTracking().FirstOrDefaultAsync(x => x.Spz == normalizedSpz);
        
        if (vehicle == null)
        {
            passage.CalculatedFee = 0.0m;
            passage.VehicleId = null;

            CreateViolation(
                passage,
                ViolationTypeCode.UnregisteredVehicle,
                $"Vehicle with detected SPZ '{normalizedSpz}' is not registered in the system.",
                5000.0m
            );




            _db.Passages.Add(passage);
            await _db.SaveChangesAsync();

            return PassageResponseDtoFromPassage(passage, normalizedSpz);
        }




        passage.VehicleId = vehicle.VehicleId;
        passage.CalculatedFee = CalculateTollFee(vehicle);
        _db.Passages.Add(passage);

        var isVignetteValid = await _vignetteService.CheckVignetteAsync(vehicle, passage.Timestamp);
        var (isInspectionValid, isEmissionValid) = await _vehicleInspectionService.IsInspectionAndEmissionValidAsync(vehicle, passage.Timestamp);



        if (!isVignetteValid)
        {
            CreateViolation(
                passage,
                ViolationTypeCode.NoVignette,
                "Vehicle is missing valid vignette.",
                1500.0m);
        }

        if (!isInspectionValid)
        {
            CreateViolation(
                passage,
                ViolationTypeCode.ExpiredVehicleInspection,
                "Vehicle has expired technical inspection.",
                2000.0m);
        }
        if (!isEmissionValid)
        {
            CreateViolation(
                passage,
                ViolationTypeCode.ExpiredEmission,
                "Vehicle has expired emissions.",
                1500.0m);
        }


        // calculating speed over the speed limit with tolerance deducted (if camera captures 150 it deducts 3% from 150 = 145,5, return 145,5 - 130 (limit for car) = 14,5 over speed limit)
        int? speedOver = _speedService.GetSpeedOverLimit(passage, vehicle);
        if (speedOver != null)
        {
            int speedLimit = vehicle.Type == VehicleType.Truck ? 90 : 130;
            decimal penalty = speedOver.Value switch
            {
                < 20 => 1000.0m,
                < 50 => 2500.0m,
                _ => 5000.0m
            };

            CreateViolation(
                passage,
                ViolationTypeCode.Speeding,
                $"Max speed for {vehicle.Type}: {speedLimit} km/h. Detected: {passage.VehicleSpeed} km/h. Exceeded by + {speedOver.Value} km/h.",
                penalty);
        }




        
        await _db.SaveChangesAsync();

        return PassageResponseDtoFromPassage(passage, normalizedSpz);
    }

    private decimal CalculateTollFee(Vehicle vehicle)
    {
        return vehicle.Type switch
        {
            VehicleType.Truck => 150.0m,
            VehicleType.Other => 100.0m,
            _ => 0.0m
        };
    }

    private void CreateViolation(Passage passage, ViolationTypeCode typeCode, string details, decimal penaltyAmount)
    {
        passage.TrafficViolations.Add(new TrafficViolation
        {
            ViolationType = typeCode,
            Details = details,
            ActualPenaltyAmount = penaltyAmount
        });
        
    }


    private static PassageResponseDto PassageResponseDtoFromPassage(Passage passage, string detectedSpz)
    {
        return new PassageResponseDto
        {
            PassageId = passage.PassageId,
            DetectedSpz = detectedSpz,
            GateId = passage.GateId,
            VehicleId = passage.VehicleId,
            VehicleSpeed = passage.VehicleSpeed,
            CalculatedFee = passage.CalculatedFee,
            Timestamp = passage.Timestamp,
            HasViolations = passage.TrafficViolations.Count > 0
        };
    }
}