using HighwayTollsystem.Models;

namespace HighwayTollsystem.Interfaces
{
    public interface IVehicleInspectionService
    {
        Task<(bool IsInspectionValid, bool IsEmissionValid)> IsInspectionAndEmissionValidAsync(Vehicle vehicle, DateTime passageTime);
    }
}
