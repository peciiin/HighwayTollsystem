using HighwayTollsystem.DTOs;
namespace HighwayTollsystem.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleResponseDto>> GetVehiclesAsync(VehicleFilterDto filter);

        Task<VehicleResponseDto?> GetVehicleByIdAsync(long vehicleId);

        Task<VehicleResponseDto?> CreateVehicleAsync(RegisterNewVehicleDto dtoVehicle);
    }
}
