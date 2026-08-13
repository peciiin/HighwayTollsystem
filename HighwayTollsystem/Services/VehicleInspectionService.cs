using HighwayTollsystem.Enums;
using HighwayTollsystem.Interfaces;
using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HighwayTollsystem.Services
{
    public class VehicleInspectionService : IVehicleInspectionService
    {
        private readonly HighwayTollContext _db;
        public VehicleInspectionService(HighwayTollContext db)
        {
            _db = db;
        }
        // stk and emission


        public async Task<(bool IsInspectionValid, bool IsEmissionValid)> IsInspectionAndEmissionValidAsync(Vehicle vehicle, DateTime passageTime)
        {
            bool isEmissionValid = vehicle.FuelType == FuelType.Electric;
            var latestInspection = await _db.VehicleInspections.AsNoTracking().Where(s => s.VehicleId == vehicle.VehicleId).OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestInspection == null) return (false, isEmissionValid);
            
            bool isInspectionValid = latestInspection.ValidTo >= passageTime;
            if (!isEmissionValid) isEmissionValid = latestInspection.EmissionsValidTo >= passageTime;
            return (isInspectionValid, isEmissionValid);
        }
    }

}
