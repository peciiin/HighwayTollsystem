using HighwayTollsystem.Models;
using Microsoft.AspNetCore.Components.Web;
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

        public async Task<bool> CheckVignetteAsync(Vehicle vehicle, DateTime passGateTime)
        {
            if (vehicle.Type.TypeName == "TRUCK")
            {
                return true;
            }

            var validVignette = await _db.Vignettes.FirstOrDefaultAsync(x => x.Spz == vehicle.Spz && x.ValidFrom <= passGateTime && x.ValidTo >= passGateTime);
            return validVignette != null;
        }
    }
}
