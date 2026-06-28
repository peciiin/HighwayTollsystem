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

        public async Task ProcessPassageAsync(Passage passage)
        {
            var vehicle = await _db.Vehicles
                .Include(v => v.Type)
                .FirstOrDefaultAsync(v => v.Spz == passage.Spz);



            passage.IsVignetteValid = true;

            // Uložíme průjezd do databáze
            _db.Passages.Add(passage);
            await _db.SaveChangesAsync();
            int nadlimit = passage.VehicleSpeed - 130;

            if (nadlimit > 0)
            {
                string violationCode = nadlimit switch
                {
                    >= 50 => "SPEED_HIGH",
                    >= 20 => "SPEED_MEDIUM",
                    _ => "SPEED_LOW"
                };

                var violationType = await _db.ViolationTypes
                    .FirstOrDefaultAsync(vt => vt.Code == violationCode);

                if (violationType != null)
                {
                    var violation = new TrafficViolation
                    {
                        PassageId = passage.PassageId,
                        ViolationTypeId = violationType.ViolationTypeId,
                        Details = "Speed exceded",
                        ActualPenaltyAmount = violationType.DefaultPenaltyAmount
                    };

                    _db.TrafficViolations.Add(violation);
                    await _db.SaveChangesAsync();
                }
            }
        }
    }
}