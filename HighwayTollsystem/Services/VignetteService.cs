using HighwayTollsystem.Enums;
using HighwayTollsystem.Interfaces;
using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HighwayTollsystem.Services;

public class VignetteService : IVignetteService
{
    private readonly HighwayTollContext _db;

    public VignetteService(HighwayTollContext db)
    {
        _db = db;
    }

    public async Task<bool> CheckVignetteAsync(Vehicle vehicle, DateTime passGateTime)
    {
        if (vehicle.Type == VehicleType.Truck || vehicle.Type == VehicleType.Motorcycle || vehicle.Type == VehicleType.Other) return true;

        if (vehicle.EmissionClass == EmissionClass.EV) return true;

        return await _db.Vignettes.AsNoTracking().AnyAsync(x => x.VehicleId == vehicle.VehicleId
            && x.ValidFrom <= passGateTime
            && x.ValidTo >= passGateTime
            );
    }
}