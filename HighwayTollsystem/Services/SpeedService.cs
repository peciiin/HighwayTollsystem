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

        public async Task CheckSpeedViolationAsync(Passage passage, Vehicle vehicle)
        {
            var speed = passage.VehicleSpeed;
            if (speed <= 100) speed -= 3;
            else
            {
                speed = (int)(Math.Floor(speed * 0.97));
            }

            var speedLimit = vehicle.Type.TypeName == "TRUCK" ? 90 : 130;
            var speedOver = passage.VehicleSpeed - speedLimit;

            if (speedOver <= 0) return;

            string code = speedOver switch
            {
                < 20 => "SPEED_LOW",
                < 50 => "SPEED_MEDIUM",
                _ => "SPEED_HIGH"
            };

            var violationType = await _db.ViolationTypes
                .FirstOrDefaultAsync(v => v.Code == code);

            if (violationType != null)
            {
                var violation = new TrafficViolation
                {
                    PassageId = passage.PassageId,
                    ViolationTypeId = violationType.ViolationTypeId,
                    Details = $"Max speed for {vehicle.Type?.TypeName}: {speedLimit} km/h. " +
                              $"Detected: {passage.VehicleSpeed} km/h. " +
                              $"After tolerance: {speed} km/h (Violation by +{speedOver} km/h).",
                    ActualPenaltyAmount = violationType.DefaultPenaltyAmount
                };
                _db.TrafficViolations.Add(violation);
                await _db.SaveChangesAsync();
            }
            
        }
    }
}
