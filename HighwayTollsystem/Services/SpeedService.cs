using HighwayTollsystem.Models;
using HighwayTollsystem.Enums;
using Microsoft.EntityFrameworkCore;

namespace HighwayTollsystem.Services
{

    public class SpeedService : ISpeedService
    {
        private readonly HighwayTollContext _db;

        public SpeedService(HighwayTollContext db)
        {
            _db = db;
        }

        public int? GetSpeedOverLimit(Passage passage, Vehicle vehicle)
        {
            int speed = passage.VehicleSpeed;
            int speedLimit = vehicle.Type == VehicleType.Truck ? 90 : 130;

            if (speed <= 100) speed -= 3;
            else speed = (int)Math.Floor(speed * 0.97);

            return (speed - speedLimit) > 0 ? speed - speedLimit : null;


        }
    }
}
