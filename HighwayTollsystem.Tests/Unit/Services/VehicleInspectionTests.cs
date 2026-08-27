using FluentAssertions;
using HighwayTollsystem.Enums;
using HighwayTollsystem.Models;
using HighwayTollsystem.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HighwayTollsystem.Tests.Unit.Services
{
    public class VehicleInspectionTests
    {
        private readonly HighwayTollContext _context;
        private readonly VehicleInspectionService _sut;
        public VehicleInspectionTests()
        {
            var options = new DbContextOptionsBuilder<HighwayTollContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new HighwayTollContext(options);
            _sut = new VehicleInspectionService(_context);
        }

        [Fact]
        public async Task IsInspectionAndEmissionValidAsync_ElectricVehicleWithZeroRecords_ReturnsFalseInspectionAndTrueEmission()
        {
            var vehicle = new Vehicle
            {
                VehicleId = 1,
                FuelType = FuelType.Electric
            };

            var (isInspectionValid, isEmissionValid) = await _sut.IsInspectionAndEmissionValidAsync(vehicle, DateTime.UtcNow);

            isInspectionValid.Should().BeFalse();
            isEmissionValid.Should().BeTrue();
        }
    }
}
