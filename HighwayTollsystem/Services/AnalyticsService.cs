using HighwayTollsystem.DTOs;
using HighwayTollsystem.Enums;
using HighwayTollsystem.Interfaces;
using HighwayTollsystem.Models;
using HighwayTollsystem.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HighwayTollsystem.Services
{

    public class AnalyticsService : IAnalyticsService
    {
        private readonly HighwayTollContext _db;

        public AnalyticsService(HighwayTollContext db)
        {
            _db = db;
        }

        public async Task<AnalyticsDashboardDto> GetDashboardAsync()
        {
            

            var personalCarsQuery = _db.Passages.AsNoTracking()
                .Where(p => p.VehicleId != null && p.Vehicle != null && p.Vehicle.Type == VehicleType.Car);
            var totalPersonalCars = await personalCarsQuery.CountAsync();

            var totalPassages = await _db.Passages.AsNoTracking().CountAsync();

            var totalPersonalCarsUnique = await personalCarsQuery.Select(p => p.VehicleId).Distinct().CountAsync();




            var totalViolations = await _db.TrafficViolations.AsNoTracking().CountAsync();

            var totalPenaltyAmount = await _db.TrafficViolations
                .AsNoTracking()
                .SumAsync(x => (decimal?)x.ActualPenaltyAmount) ?? 0.0m;

            return new AnalyticsDashboardDto
            {
                TotalPassages = totalPassages,
                TotalViolations = totalViolations,
                TotalPenaltyAmount = totalPenaltyAmount,
                TotalPersonalCars = totalPersonalCars,
                TotalPersonalCarsUnique = totalPersonalCarsUnique
            };
        }
        // returns most used gates on highway
        public async Task<List<AnalyticsGateStatsDto>> GetTopGatesAsync(int count = 5)
        {
            if (count <= 0) count = 5;
            else if (count > 100) count = 100;

            return await _db.Passages.AsNoTracking().GroupBy(p => p.GateId)
                    .Select(g => new AnalyticsGateStatsDto
                    {
                        GateId = g.Key,
                        PassageCount = g.Count(),
                        TotalFeesCollected = g.Sum(p => p.CalculatedFee)
                    })
                    .OrderByDescending(g => g.PassageCount)
                    .Take(count)
                    .ToListAsync();
        }

        // returns ordered violations from high to low with their count and total ammount of fines
        public async Task<List<AnalyticsViolationsStatsDto>> GetViolationTypeBreakdownAsync()
        {
            return await _db.TrafficViolations.AsNoTracking()
                .GroupBy(v => v.ViolationType)
                .Select(g => new AnalyticsViolationsStatsDto
                {
                    ViolationCode = g.Key,
                    Count = g.Count(),
                    TotalFines = g.Sum(v => (decimal?)v.ActualPenaltyAmount ?? 0.0m)
                })
                .OrderByDescending(g => g.Count)
                .ToListAsync();
        }




        
    }
}
