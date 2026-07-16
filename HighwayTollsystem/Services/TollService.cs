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

            var vehicle = await _db.Vehicles.Include(t => t.Type).FirstOrDefaultAsync(x => x.Spz == passage.Spz);
            if (vehicle == null)
            {
                
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
