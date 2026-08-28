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
        public async Task IsInspectionAndEmissionValidAsync_ElectricWithoutRecords_ReturnsFalseInspectionAndTrueEmission()
        {
            var vehicle = new Vehicle 
            {
                VehicleId = 1, FuelType = FuelType.Electric 
            };


            var (isInspectionValid, isEmissionValid) = await _sut.IsInspectionAndEmissionValidAsync(vehicle, DateTime.UtcNow);

            isInspectionValid.Should().BeFalse();
            isEmissionValid.Should().BeTrue();
        }



        [Fact]
        public async Task IsInspectionAndEmissionValidAsync_ElectricWithRecord_ReturnsTrueInspectionAndTrueEmission()
        {
            var vehicle = new Vehicle { 
                VehicleId = 1, FuelType = FuelType.Electric 
            };
            _context.VehicleInspections.Add(
                new VehicleInspection
                {
                    VehicleInspectionId = 1,
                    VehicleId = 1,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1),
                    ValidTo = DateTime.UtcNow.AddYears(1)
                });



            await _context.SaveChangesAsync();
            var (isInspectionValid, isEmissionValid) = await _sut.IsInspectionAndEmissionValidAsync(vehicle, DateTime.UtcNow);
            
            isInspectionValid.Should().BeTrue();
            isEmissionValid.Should().BeTrue();
        }



        [Fact]
        public async Task IsInspectionAndEmissionValidAsync_GasolineWithoutRecords_ReturnsBothFalse()
        {
            var vehicle = new Vehicle
            {
                VehicleId = 2, FuelType = FuelType.Petrol 
            };

            var (isInspectionValid, isEmissionValid) = await _sut.IsInspectionAndEmissionValidAsync(vehicle, DateTime.UtcNow);

            isInspectionValid.Should().BeFalse();
            isEmissionValid.Should().BeFalse();
        }



        [Fact]
        public async Task IsInspectionAndEmissionValidAsync_ValidRecords_ReturnsBothTrue()
        {
            var passageTime = DateTime.UtcNow;
            var vehicle = new Vehicle { VehicleId = 3, FuelType = FuelType.Diesel };


            _context.VehicleInspections.Add(new VehicleInspection
            {
                VehicleInspectionId = 1,
                VehicleId = 3,
                CreatedAt = passageTime.AddMonths(-1),
                ValidTo = passageTime.AddYears(1),
                EmissionsValidTo = passageTime.AddYears(1)
            });
            await _context.SaveChangesAsync();

            var (isInspectionValid, isEmissionValid) = await _sut.IsInspectionAndEmissionValidAsync(vehicle, passageTime);

            isInspectionValid.Should().BeTrue();
            isEmissionValid.Should().BeTrue();
        }



        [Fact]
        public async Task IsInspectionAndEmissionValidAsync_ExpiredInspectionAndValidEmissions_ReturnsFalseAndTrue()
        {
            var passageTime = DateTime.UtcNow;
            var vehicle = new Vehicle { VehicleId = 4, FuelType = FuelType.Petrol };



            _context.VehicleInspections.Add(
                new VehicleInspection
            {
                VehicleInspectionId = 2,
                VehicleId = 4,
                CreatedAt = passageTime.AddYears(-2),
                ValidTo = passageTime.AddDays(-1),
                EmissionsValidTo = passageTime.AddMonths(6)
            });
            await _context.SaveChangesAsync();

            var (isInspectionValid, isEmissionValid) = await _sut.IsInspectionAndEmissionValidAsync(vehicle, passageTime);

            isInspectionValid.Should().BeFalse();
            isEmissionValid.Should().BeTrue();
        }

        [Fact]
        public async Task IsInspectionAndEmissionValidAsync_MultipleRecords_UsesLatestByCreatedAt()
        {

            var passageTime = DateTime.UtcNow;
            var vehicle = new Vehicle 
            {
                VehicleId = 5,
                FuelType = FuelType.Diesel 
            };



            _context.VehicleInspections.AddRange(
                new VehicleInspection
                {
                    VehicleInspectionId = 10,
                    VehicleId = 5,
                    CreatedAt = passageTime.AddYears(-2),
                    ValidTo = passageTime.AddDays(-10),
                    EmissionsValidTo = passageTime.AddDays(-10)
                },


                new VehicleInspection
                {
                    VehicleInspectionId = 11,
                    VehicleId = 5,
                    CreatedAt = passageTime.AddDays(-5),
                    ValidTo = passageTime.AddYears(2),
                    EmissionsValidTo = passageTime.AddYears(2)
                }
            );
            await _context.SaveChangesAsync();

            var (isInspectionValid, isEmissionValid) = await _sut.IsInspectionAndEmissionValidAsync(vehicle, passageTime);

            isInspectionValid.Should().BeTrue();
            isEmissionValid.Should().BeTrue();
        }
    }
}
