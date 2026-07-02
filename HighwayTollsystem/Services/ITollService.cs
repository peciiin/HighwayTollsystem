using HighwayTollsystem.Models;

namespace HighwayTollsystem.Services
{
    public interface ITollService
    {
        Task PassageProcessingAsync(Passage passage);
    }
}
