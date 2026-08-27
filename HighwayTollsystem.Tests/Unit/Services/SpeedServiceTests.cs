using HighwayTollsystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using HighwayTollsystem.Enums;
using HighwayTollsystem.Models;

namespace HighwayTollsystem.Tests.Unit.Services
{
    public class SpeedServiceTests
    {
        private readonly HighwayTollContext _context;
        private readonly SpeedService _sut;
        public SpeedServiceTests() 
        {
            var options = new DbContextOptionsBuilder<HighwayTollContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new HighwayTollContext(options);
            _sut = new SpeedService(_context);
        }

        [Theory]
        [InlineData(100, VehicleType.Car, null)]
        [InlineData(105, VehicleType.Car, null)]
        [InlineData(134, VehicleType.Car, null)]
        [InlineData(140, VehicleType.Car, 5)]
        [InlineData(150, VehicleType.Car, 15)]
        [InlineData(80, VehicleType.Truck, null)]
        [InlineData(90, VehicleType.Truck, null)]
        [InlineData(93, VehicleType.Truck, null)]
        [InlineData(100, VehicleType.Truck, 7)]
        [InlineData(120, VehicleType.Truck, 26)]
        public void GetSpeedOverLimit_SpeedOverLimitCalculation(int speed, VehicleType vehicleType, int? expectedOverLimit)
        {
            var vehicle = new Vehicle { Type = vehicleType };
            var passage = new Passage { VehicleSpeed = speed };

            var result = _sut.GetSpeedOverLimit(passage, vehicle);

            result.Should().Be(expectedOverLimit);
        }


    }
}
