using HighwayTollsystem.Models;
using Microsoft.EntityFrameworkCore;


namespace HighwayTollsystem.Services
{

    public class VignetteService
    {
        private readonly HighwayTollContext _db;

        public VignetteService(HighwayTollContext db)
        {
            _db = db;
        }

        public async Task<bool> IsVignetteValidAsync(Vehicle vehicle, DateTime passageTime)
        {
            
            if (vehicle.Type.TypeName == "TRUCK")
            {
                return true;
            }

            
            var activeVignette = await _db.Vignettes
                .FirstOrDefaultAsync(v => v.Spz == vehicle.Spz &&
                                          v.ValidFrom <= passageTime &&
                                          v.ValidTo >= passageTime);

            return activeVignette != null;
        }
    }
}
