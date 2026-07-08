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

        public async Task<bool> IsStkValidAsync(Vehicle vehicle, DateTime passageTime)
        {
            var activeStk = await _db.Stks
                .FirstOrDefaultAsync(s => s.Spz == vehicle.Spz &&
                                          s.ValidTo >= passageTime &&
                                          s.EmissionsValidTo >= passageTime);

            return activeStk != null;
        }
    }
}
