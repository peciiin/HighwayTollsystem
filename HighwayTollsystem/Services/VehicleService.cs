using HighwayTollsystem.DTOs;
using HighwayTollsystem.Interfaces;
using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HighwayTollsystem.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly HighwayTollContext _db;
        public VehicleService(HighwayTollContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<VehicleResponseDto>> GetVehiclesAsync(VehicleFilterDto filter)
        {
            var query = _db.Vehicles.AsNoTracking().AsQueryable();
            if (filter.FuelType.HasValue)
            {
                query = query.Where(v => v.FuelType == filter.FuelType.Value);
            }
            if (filter.VehicleType.HasValue)
            {
                query = query.Where(v => v.Type == filter.VehicleType.Value);
            }
            if (!string.IsNullOrWhiteSpace(filter.Spz))
            {
                var spz = filter.Spz.ToUpper();
                query = query.Where(v => v.Spz.Contains(spz));
            }
            if (filter.RegisteredFrom.HasValue)
            {
                query = query.Where(v => v.RegisteredAt >= filter.RegisteredFrom.Value);
            }
            if (filter.RegisteredTo.HasValue)
            {
                query = query.Where(v => v.RegisteredAt <= filter.RegisteredTo.Value);
            }
            if (!string.IsNullOrEmpty(filter.CountryCode))
            {
                query = query.Where(v => v.CountryCode == filter.CountryCode);
            }
            if (!string.IsNullOrWhiteSpace(filter.Vin))
            {
                var vinUpper = filter.Vin.ToUpper();
                query = query.Where(v => v.Vin != null && v.Vin.Contains(vinUpper));
            }
            if (filter.EmissionClass.HasValue)
            {
                query = query.Where(v => v.EmissionClass == filter.EmissionClass.Value);
            }

            int pageSize = filter.PageSize > 0 ? Math.Min(filter.PageSize, 100) : 10;
            int pageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
            int skip = (pageNumber - 1) * pageSize;

            var vehicles = await query.OrderByDescending(v => v.RegisteredAt).Skip(skip).Take(pageSize)
                .Select(v => new VehicleResponseDto
                {
                    VehicleId = v.VehicleId,
                    Spz = v.Spz,
                    Type = v.Type,
                    FuelType = v.FuelType,
                    EmissionClass = v.EmissionClass,
                    CountryCode = v.CountryCode,
                    Vin = v.Vin,
                    RegisteredAt = v.RegisteredAt
                }).ToListAsync();

            return vehicles;
        }

        public async Task<VehicleResponseDto?> CreateVehicleAsync(RegisterNewVehicleDto dtoVehicle)
        {
            var spz = dtoVehicle.Spz.Trim().ToUpper();

            var exists = await _db.Vehicles.AnyAsync(v => v.Spz == spz);
            if (exists) return null;

            var vehicle = new Vehicle
            {
                Spz = spz,
                Type = dtoVehicle.Type,
                FuelType = dtoVehicle.FuelType,
                EmissionClass = dtoVehicle.EmissionClass,
                CountryCode = string.IsNullOrWhiteSpace(dtoVehicle.CountryCode) ? "CZ" : dtoVehicle.CountryCode.Trim().ToUpper(),
                Vin = dtoVehicle.Vin?.Trim().ToUpper(),
                RegisteredAt = DateTime.UtcNow
            };

            _db.Vehicles.Add(vehicle);
            await _db.SaveChangesAsync();

            return new VehicleResponseDto
            {
                VehicleId = vehicle.VehicleId,
                Spz = vehicle.Spz,
                Type = vehicle.Type,
                FuelType = vehicle.FuelType,
                EmissionClass = vehicle.EmissionClass,
                CountryCode = vehicle.CountryCode,
                Vin = vehicle.Vin,
                RegisteredAt = vehicle.RegisteredAt
            };
        }


        public async Task<VehicleResponseDto?> GetVehicleByIdAsync(long vehicleId)
        {
            return await _db.Vehicles.AsNoTracking().Where(v => v.VehicleId == vehicleId).Select(v => new VehicleResponseDto
                {
                    VehicleId = v.VehicleId,
                    Spz = v.Spz,
                    Type = v.Type,
                    FuelType = v.FuelType,
                    EmissionClass = v.EmissionClass,
                    CountryCode = v.CountryCode,
                    Vin = v.Vin,
                    RegisteredAt = v.RegisteredAt
                }).FirstOrDefaultAsync();
        }
    }
}
