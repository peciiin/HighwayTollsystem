using HighwayTollsystem.DTOs;
using HighwayTollsystem.Models;

namespace HighwayTollsystem.Interfaces
{
    public interface ITollService
    {
        Task<PassageResponseDto?> PassageProcessingAsync(RegisterTollPassDto registerDto);
    }
}
