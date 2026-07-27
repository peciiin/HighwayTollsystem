using HighwayTollsystem.Models;
using HighwayTollsystem.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HighwayTollsystem.Services
{
    
    public class AnalyticsService
    {
        private readonly HighwayTollContext _db;

        public AnalyticsService(HighwayTollContext db)
        {
            _db = db;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var totalPassages = await _db.Passages.CountAsync();


            var countCar = await _db.Passages
            .Where(p => p.Spz != "UNKNOWN"
                     && p.SpzNavigation != null
                     && p.SpzNavigation.Type.TypeName == "PERSONAL").Select(p => p.Spz).ToListAsync();

            var totalPersonalCars = countCar.Count;
            var totalPersonalCarsUnique = countCar.Distinct().Count();



            var totalViolations = await _db.TrafficViolations.CountAsync();
            var totalPenatlyAmmount = await _db.TrafficViolations.SumAsync(x => (decimal?)x.ActualPenaltyAmount) ?? 0.0m;

            return new DashboardDto
            {
                TotalPassages = totalPassages,

                TotalViolations = totalViolations,
                TotalPenaltyAmount = totalPenatlyAmmount,

                TotalPersonalCars = totalPersonalCars,
                TotalPersonalCarsUnique = totalPersonalCarsUnique
            };
        }
        // returns most used gates on highway
        public async Task<List<GateStatDto>> GetTopGatesAsync(int count = 5)
        {
            return await _db.Passages
                .GroupBy(p => p.GateId)
                .Select(g => new GateStatDto
                {
                    GateId = g.Key,
                    PassageCount = g.Count(),
                    TotalFeesCollected = g.Sum(p => p.CalculatedFee)
                }).OrderByDescending(g => g.PassageCount).Take(count).ToListAsync();
        }

        // returns ordered violations from high to low with their count and total ammount of fines
        public async Task<List<ViolationStatDto>> GetViolationTypeBreakdownAsync()
        {
            return await _db.TrafficViolations
                .Include(v => v.ViolationType).GroupBy(v => v.ViolationType.Code)
                .Select(g => new ViolationStatDto
                {
                    ViolationCode = g.Key ?? "UNKNOWN",
                    Count = g.Count(),
                    TotalFines = g.Sum(v => v.ActualPenaltyAmount)
                }).OrderByDescending(v => v.Count).ToListAsync();
        }




        public class GateStatDto
        {
            public int GateId { get; set; }
            public int PassageCount { get; set; }
            public decimal TotalFeesCollected { get; set; }
        }



        public class ViolationStatDto
        {
            public string ViolationCode { get; set; } = null!;
            public int Count { get; set; }
            public decimal TotalFines { get; set; }
        }



        public class DashboardDto
        {
            public int TotalPassages { get; set; }
            public int TotalViolations { get; set; }
         
            public decimal TotalPenaltyAmount { get; set; }
            public int TotalPersonalCars { get; set; }
            public int TotalPersonalCarsUnique { get; set; }

        }
    }
}
