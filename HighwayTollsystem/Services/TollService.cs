using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;
namespace HighwayTollsystem.Services
{
    public class TollService
    { //fix
        private readonly HighwayTollContext _db;
        private readonly VignetteService _vignetteService;
        private readonly SpeedService _speedService;
        private readonly StkService _stkService;
        public TollService(HighwayTollContext db, VignetteService vignetteService, SpeedService speedService, StkService stkService)
        {
            _db = db;
            _vignetteService = vignetteService;
            _speedService = speedService;
            _stkService = stkService;
        }

        public async Task PassageProcessingAsync(Passage passage)
        {

            var vehicle = await _db.Vehicles.Include(t => t.Type).FirstOrDefaultAsync(x => x.Spz == passage.Spz);

            // if vehicle is unregistered
            if (vehicle == null)
            {
                string originalSpz = passage.Spz;


                passage.Spz = "UNKNOWN";
                passage.CalculatedFee = 0.0m;
                passage.IsVignetteValid = false;



                _db.Passages.Add(passage);
                await _db.SaveChangesAsync();



                await CreateViolationAsync(passage, "UNREGISTERED_VEHICLE",
                    $"Vehicle with detected SPZ '{originalSpz}' is not registered in the system!");
                return;
            }

            if (vehicle.Type?.TypeName == "TRUCK")
            {
                passage.CalculatedFee = vehicle.Type.BaseTarif ?? 150.0m;
            }
            else
            {
                passage.CalculatedFee = 0.0m;
            }

            bool isVignetteValid = await _vignetteService.CheckVignetteAsync(vehicle, passage.Timestamp);
            passage.IsVignetteValid = isVignetteValid;

            _db.Passages.Add(passage);
            await _db.SaveChangesAsync();


            // vignette check and violation
            if (isVignetteValid)
            {
                await CreateViolationAsync(passage, "MISSING_VIGNETTE", $"Vehicle is missing vignette");
            }


            // speed check and violation
            int? speedOver = _speedService.GetSpeedOverLimit(passage, vehicle);
            if (speedOver != null)
            {
                string speedCode = speedOver.Value switch
                {
                    < 20 => "SPEED_LOW",
                    < 50 => "SPEED_MEDIUM",
                    _ => "SPEED_HIGH"
                };

                int speedLimit = vehicle.Type?.TypeName == "TRUCK" ? 90 : 130;

                await CreateViolationAsync(passage, speedCode,
                    $"Max speed for {vehicle.Type?.TypeName}: {speedLimit} km/h. " +
                    $"Detected: {passage.VehicleSpeed} km/h. " +
                    $"Violation by +{speedOver.Value} km/h (after tolerance).");
            }

            // stk check and violation
            bool isStkValid = await _stkService.IsStkValidAsync(vehicle, passage.Timestamp);
            if (!isStkValid)
            {
                await CreateViolationAsync(passage, "EXPIRED_STK", "Vehicle has expired stk.");
            }

            // emission check and violation
            bool isEmissionsValid = await _stkService.IsEmisionValidAsync(vehicle, passage.Timestamp);
            if (!isEmissionsValid)
            {
                await CreateViolationAsync(passage, "EMISSION_FAILURE", "Vehicle has expired emissions.");
            }
        }

        private async Task CreateViolationAsync(Passage passage, string violationTypeCode, string details)
        {
            var violationType = await _db.ViolationTypes
                .FirstOrDefaultAsync(v => v.Code == violationTypeCode);

            if (violationType != null)
            {
                var violation = new TrafficViolation
                {
                    PassageId = passage.PassageId,
                    ViolationTypeId = violationType.ViolationTypeId,
                    Details = details,
                    ActualPenaltyAmount = violationType.DefaultPenaltyAmount
                };

                _db.TrafficViolations.Add(violation);
                await _db.SaveChangesAsync();
            }
        }
    }
}
