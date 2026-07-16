using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HighwayTollsystem.Services
{
    
    public class SpeedService
    {
        private readonly HighwayTollContext _db;

        public SpeedService(HighwayTollContext db)
        {
            _db = db;
        }

        public int? GetSpeedOverLimit(Passage passage, Vehicle vehicle)
        {
            int speedTolerance = passage.VehicleSpeed;
            if (speedTolerance <= 100) speedTolerance -= 3;
            else speedTolerance = (int)Math.Floor(speedTolerance * 0.97);


            


            if (speedTolerance <= 0) return null;
            return speedTolerance;

            
            
        }
    }
}
