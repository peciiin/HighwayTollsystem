using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HighwayTollsystem.Services
{
    public class StkService
    {
        private readonly HighwayTollContext _db;
        public StkService(HighwayTollContext db)
        {
            _db = db;
        }
        // stk
        public async Task<bool> IsStkValidAsync(Vehicle vehicle, DateTime passageTime)
        {
            var latestStk = await _db.Stks
                .Where(s => s.Spz == vehicle.Spz)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (latestStk == null) return false;



            return latestStk.ValidTo >= passageTime;
        }

        // emission
        public async Task<bool> IsEmisionValidAsync(Vehicle vehicle, DateTime passageTime)
        {
            var latestStk = await _db.Stks
                .Where(s => s.Spz == vehicle.Spz)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();
            
            if (latestStk == null) return false;



            return latestStk.EmissionsValidTo >= passageTime;
        }
    }
}