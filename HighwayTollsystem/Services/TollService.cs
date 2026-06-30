using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;
namespace HighwayTollsystem.Services
{
    public class TollService
    {
        private readonly HighwayTollContext _db;
        private readonly VignetteService _vignetteService;
        public TollService(HighwayTollContext db, VignetteService vignetteService)
        {
            _db = db;
            _vignetteService = vignetteService;
        }

        public async Task PassageProcessingAsync(Passage passage)
        {
            var vehicle = await _db.Vehicles.Include(t => t.Type).FirstOrDefaultAsync(x => x.Spz == passage.Spz);
            if (vehicle == null)
            {
                return;
            }
            CalculateRoadFee(passage, vehicle);
            passage.IsVignetteValid = await _vignetteService.IsVignetteValidAsync(vehicle, passage.Timestamp);

            
            _db.Passages.Add(passage);
            await _db.SaveChangesAsync();
            if (!passage.IsVignetteValid)
            {
                await CreateMissingVignetteViolationAsync(passage);
            }

            await CheckViolations(passage);


        }

        public void CalculateRoadFee(Passage passage, Vehicle vehicle)
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

        private async Task CreateMissingVignetteViolationAsync(Passage passage)
        {
            var violationType = await _db.ViolationTypes
                .FirstOrDefaultAsync(v => v.Code == "MISSING_VIGNETTE");

            if (violationType != null)
            {
                var violation = new TrafficViolation
                {
                    PassageId = passage.PassageId,
                    ViolationTypeId = violationType.ViolationTypeId,
                    Details = "Vehicle is missing a valid vignette.",
                    ActualPenaltyAmount = violationType.DefaultPenaltyAmount
                };
                _db.TrafficViolations.Add(violation);
                await _db.SaveChangesAsync();
            }
        }
    }
}
