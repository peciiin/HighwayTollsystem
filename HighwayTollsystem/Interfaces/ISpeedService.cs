using HighwayTollsystem.Models;

namespace HighwayTollsystem.Services
{
    public interface ISpeedService
    {
        int? GetSpeedOverLimit(Passage passage, Vehicle vehicle);
    }
}