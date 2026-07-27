using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HighwayTollsystem.Services
{
    public class TollService
    {
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
            var violationTypes = await _db.ViolationTypes
                .AsNoTracking()
                .ToDictionaryAsync(v => v.Code, v => v);





            var vehicle = await _db.Vehicles
                .AsNoTracking()
                .Include(t => t.Type)
                .FirstOrDefaultAsync(x => x.Spz == passage.Spz);



            if (vehicle == null)
            {
                string originalSpz = passage.Spz;

                passage.Spz = "UNKNOWN";
                passage.CalculatedFee = 0.0m;
                passage.IsVignetteValid = false;

                _db.Passages.Add(passage);

                CreateViolation(passage, "UNREGISTERED_VEHICLE",
                    $"Vehicle with detected SPZ '{originalSpz}' is not registered in the system!", violationTypes);

                await _db.SaveChangesAsync();
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

            if (!isVignetteValid)
            {
                CreateViolation(passage, "MISSING_VIGNETTE", "Vehicle is missing vignette", violationTypes);
            }

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

                CreateViolation(passage, speedCode,
                    $"Max speed for {vehicle.Type?.TypeName}: {speedLimit} km/h. " +
                    $"Detected: {passage.VehicleSpeed} km/h. " +
                    $"Violation by +{speedOver.Value} km/h (after tolerance).", violationTypes);
            }

            bool isStkValid = await _stkService.IsStkValidAsync(vehicle, passage.Timestamp);
            if (!isStkValid)
            {
                CreateViolation(passage, "EXPIRED_STK", "Vehicle has expired stk.", violationTypes);
            }

            bool isEmissionsValid = await _stkService.IsEmisionValidAsync(vehicle, passage.Timestamp);
            if (!isEmissionsValid)
            {
                CreateViolation(passage, "EMISSION_FAILURE", "Vehicle has expired emissions.", violationTypes);
            }

            await _db.SaveChangesAsync();
        }

        private void CreateViolation(Passage passage, string violationTypeCode, string details, Dictionary<string, ViolationType> violationTypes)
        {
            if (violationTypes.TryGetValue(violationTypeCode, out var violationType))
            {
                var violation = new TrafficViolation
                {
                    Passage = passage,
                    ViolationTypeId = violationType.ViolationTypeId,
                    Details = details,
                    ActualPenaltyAmount = violationType.DefaultPenaltyAmount
                };

                _db.TrafficViolations.Add(violation);
            }
        }
    }
}