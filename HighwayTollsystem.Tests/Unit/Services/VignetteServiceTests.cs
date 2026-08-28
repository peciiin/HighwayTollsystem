using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using HighwayTollsystem.Services;
using Microsoft.EntityFrameworkCore;
using HighwayTollsystem.Models;
using FluentAssertions;
using HighwayTollsystem.Enums;


namespace HighwayTollsystem.Tests.Unit.Services
{
    public class VignetteServiceTests
    {
        private readonly HighwayTollContext _context;
        private readonly VignetteService _sut;
        public VignetteServiceTests()
        {
            var options = new DbContextOptionsBuilder<HighwayTollContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HighwayTollContext(options);
            _sut = new VignetteService(_context);
        }


        [Theory]
        [InlineData(VehicleType.Truck)]
        [InlineData(VehicleType.Motorcycle)]
        [InlineData(VehicleType.Other)]
        public async Task CheckVignetteAsync_TruckOrMotorcycleOrOther_ReturnsTrue(VehicleType vehicleType)
        {
            var vehicle = new Vehicle { Type = vehicleType };
            var passGateTime = DateTime.UtcNow;
            var result = await _sut.CheckVignetteAsync(vehicle, passGateTime);
            
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CheckVignetteAsync_ElectricVehicle_ReturnsTrue()
        {
            var vehicle = new Vehicle { Type = VehicleType.Car, EmissionClass = EmissionClass.EV };
            var passTime = DateTime.UtcNow;
            var res = await _sut.CheckVignetteAsync(vehicle, passTime);
            
            res.Should().BeTrue();
        }
        [Fact]
        public async Task CheckVignetteAsync_ValidVignette_ReturnsTrue()
        {
            var vehicle = new Vehicle
            {
                VehicleId = 1,
                Type = VehicleType.Car,
                EmissionClass = EmissionClass.EV
            };
            var passTime = DateTime.UtcNow;
            var vignette = new Vignette
            {
                VehicleId = vehicle.VehicleId,
                ValidFrom = passTime.AddDays(-1),
                ValidTo = passTime.AddDays(1)
            };
            await _context.SaveChangesAsync();
            var res = await _sut.CheckVignetteAsync(vehicle, passTime);
            res.Should().BeTrue();
        }
        [Fact]
        public async Task CheckVignetteAsync_CarWithExpiredVignette_ReturnsFalse()
        {
            var passTime = DateTime.UtcNow;
            var vehicle = new Vehicle
            {
                VehicleId = 2,
                Type = VehicleType.Car,
                EmissionClass = EmissionClass.Euro6
            };

            _context.Vignettes.Add(new Vignette
            {
                VignetteId = 2,
                VehicleId = vehicle.VehicleId,
                ValidFrom = passTime.AddMonths(-2),
                ValidTo = passTime.AddDays(-1)
            });
            await _context.SaveChangesAsync();

            var res = await _sut.CheckVignetteAsync(vehicle, passTime);

            res.Should().BeFalse();
        }


        [Fact]
        public async Task CheckVignetteAsync_CarWithoutVignette_ReturnsFalse()
        {
            var passTime = DateTime.UtcNow;
            var vehicle = new Vehicle
            {
                VehicleId = 3,
                Type = VehicleType.Car,
                EmissionClass = EmissionClass.Euro6
            };

            var res = await _sut.CheckVignetteAsync(vehicle, passTime);

            res.Should().BeFalse();
        }

        [Fact]
        public async Task CheckVignetteAsync_CarWithFutureVignette_ReturnsFalse()
        {
            var passTime = DateTime.UtcNow;
            var vehicle = new Vehicle
            {
                VehicleId = 4,
                Type = VehicleType.Car,
                EmissionClass = EmissionClass.Euro6
            };
            _context.Vignettes.Add(new Vignette
            {
                VignetteId = 4,
                VehicleId = vehicle.VehicleId,
                ValidFrom = passTime.AddDays(1),
                ValidTo = passTime.AddDays(10)
            });
            await _context.SaveChangesAsync();
            var res = await _sut.CheckVignetteAsync(vehicle, passTime);
            res.Should().BeFalse();
        }

        [Fact]
        public async Task CheckVignetteAsync_CarWithMultipleVignetteLastIsValid_ReturnsTrue()
        {
            var vehicle = new Vehicle
            {
                VehicleId = 5,
                Type = VehicleType.Car,
                EmissionClass = EmissionClass.Euro6
            };

            var passTime = DateTime.UtcNow;

            _context.Vignettes.AddRange(
                new Vignette
                {
                    VignetteId = 5,
                    VehicleId = vehicle.VehicleId,
                    ValidFrom = passTime.AddMonths(-2),
                    ValidTo = passTime.AddMonths(-1)
                },
                new Vignette
                {
                    VignetteId = 6,
                    VehicleId = vehicle.VehicleId,
                    ValidFrom = passTime.AddDays(-1),
                    ValidTo = passTime.AddDays(1)
                }
            );
            await _context.SaveChangesAsync();
            var res = await _sut.CheckVignetteAsync(vehicle, passTime);
            res.Should().BeTrue();
        }

    }
}
