using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;
namespace HighwayTollsystem.Services
{
    public class TollService
    {
        private readonly HighwayTollContext _db;
        public TollService(HighwayTollContext db)
        {
            _db = db;
        }

        public async Task PassageProcessingAsync(Passage passage)
        {
            var vehicle = await _db.Vehicles.Include(t => t.Type).FirstOrDefaultAsync(x => x.Spz == passage.Spz);
            if (vehicle == null)
            {
                return;
            }
            CalculateRoadFee(passage, vehicle);



        }

        public async Task CalculateRoadFee(Passage passage, Vehicle vehicle)
        {
            passage.CalculatedFee = vehicle.Type.BaseTarif ?? 0.0m;
        }

        private async Task CheckViolations(Passage passage)
        {
            int speed = passage.VehicleSpeed - 130;
            if (speed <= 0) return;

            string code = speed switch
            {
                < 20 => "SPEED LOW",
                < 50 => "SPEED MEDIUM",
                _ => "SPEED HIGH"
            };

            var violationType = await _db.ViolationTypes
                .FirstOrDefaultAsync(v => v.Code == code);

            if (violationType != null)
            {
                var violation = new TrafficViolation
                {
                    PassageId = passage.PassageId,
                    ViolationTypeId = violationType.ViolationTypeId,
                    Details = "Maximum speed: 130, violation: " + speed + " km/h",
                    ActualPenaltyAmount = violationType.DefaultPenaltyAmount
                };

                _db.TrafficViolations.Add(violation);
                await _db.SaveChangesAsync();
            }
        }
    }
}
