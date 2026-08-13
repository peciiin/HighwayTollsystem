using HighwayTollsystem.Models;

namespace HighwayTollsystem.Interfaces
{
    public interface IVignetteService
    {
        Task<bool> CheckVignetteAsync(Vehicle vehicle, DateTime passGateTime);
    }
}
