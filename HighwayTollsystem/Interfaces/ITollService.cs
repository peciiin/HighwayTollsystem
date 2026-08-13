using HighwayTollsystem.Models;

namespace HighwayTollsystem.Interfaces
{
    public interface ITollService
    {
        Task PassageProcessingAsync(Passage passage, string detectedSpz);
    }
}
