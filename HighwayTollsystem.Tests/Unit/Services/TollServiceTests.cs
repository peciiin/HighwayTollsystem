using FluentAssertions;
using HighwayTollsystem.DTOs;
using HighwayTollsystem.Enums;
using HighwayTollsystem.Interfaces;
using HighwayTollsystem.Models;
using HighwayTollsystem.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HighwayTollsystem.Tests.Unit.Services
{
    public class TollServiceTests
    {
        private readonly HighwayTollContext _context;
        private readonly TollService _sut;
        private readonly Mock<IVignetteService> _mockVignetteService;
        private readonly Mock<ISpeedService> _mockSpeedService;
        private readonly Mock<IVehicleInspectionService> _mockVehicleInspectionService;


        public TollServiceTests() 
        {
            var options = new DbContextOptionsBuilder<HighwayTollContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockSpeedService = new Mock<ISpeedService>();
            _mockVehicleInspectionService = new Mock<IVehicleInspectionService>();
            _mockVignetteService = new Mock<IVignetteService>();
            _context = new HighwayTollContext(options);


            _sut = new TollService(_context, _mockVignetteService.Object, _mockSpeedService.Object, _mockVehicleInspectionService.Object);

        }

        [Fact]
        public async Task PassageProcessingAsync_GateDoesNotExist_ReturnsNull()
        {

            var dto = new RegisterTollPassDto
            {
                TollGateId = 1000000,
                DetectedSpz = "1A16767",
                VehicleSpeed = 100
            };
            var res = await _sut.PassageProcessingAsync(dto);

            res.Should().BeNull();
        }



        [Fact]
        public async Task PassageProcessingAsync_VehicleNotRegistered_CreatesUnregisteredViolationAndSaves()
        {
            _context.TollGates.Add(
                new TollGate 
                {
                    GateId = 1,
                    HighwayName = "D1",
                    Direction = "Praha",
                });
            await _context.SaveChangesAsync();

            var dto = new RegisterTollPassDto
            {
                TollGateId = 1,
                DetectedSpz = "1A16767",
                VehicleSpeed = 100
            };

            var res = await _sut.PassageProcessingAsync(dto);

            res.Should().NotBeNull();
            res!.DetectedSpz.Should().Be("1A16767");
            res.VehicleId.Should().BeNull();
            res.CalculatedFee.Should().Be(0.0m);
            res.HasViolations.Should().BeTrue();

            var savePassages = await _context.Passages.Include(p => p.TrafficViolations).FirstOrDefaultAsync();

            savePassages.Should().NotBeNull();
            savePassages.TrafficViolations.First().ViolationType.Should().Be(ViolationTypeCode.UnregisteredVehicle);
            savePassages!.TrafficViolations.Should().HaveCount(1);
            savePassages.TrafficViolations.First().ActualPenaltyAmount.Should().Be(5000.0m);
        }


        [Theory]
        [InlineData(VehicleType.Car, 0.0)]
        [InlineData(VehicleType.Motorcycle, 0.0)]
        [InlineData(VehicleType.Truck, 150.0)]
        [InlineData(VehicleType.Other, 100.0)]
        public async Task PassageProcessingAsync_CleanPassage_CalculatesCorrectFeeAndNoViolations(VehicleType vehicleType, decimal expectedFee)
        {
            _context.TollGates.Add(
                new TollGate 
                { 
                    GateId = 10,
                    HighwayName = "D1",
                    Direction = "Praha",

                });

            _context.Vehicles.Add(
                new Vehicle 
                {
                    VehicleId = 10, 
                    Spz = "1B36565",
                    Type = vehicleType
                });



            await _context.SaveChangesAsync();



            var dto = new RegisterTollPassDto
            {
                TollGateId = 10,
                DetectedSpz = "1B36565",
                VehicleSpeed = 80
            };

            _mockVignetteService.Setup(s => s.CheckVignetteAsync(It.IsAny<Vehicle>(), It.IsAny<DateTime>())).ReturnsAsync(true);
            _mockVehicleInspectionService.Setup(s => s.IsInspectionAndEmissionValidAsync(It.IsAny<Vehicle>(), It.IsAny<DateTime>())).ReturnsAsync((true, true));
            _mockSpeedService.Setup(s => s.GetSpeedOverLimit(It.IsAny<Passage>(), It.IsAny<Vehicle>())).Returns((int?)null);

            var res = await _sut.PassageProcessingAsync(dto);

            res.Should().NotBeNull();
            res!.CalculatedFee.Should().Be(expectedFee);
            res.HasViolations.Should().BeFalse();

            var savedPassage = await _context.Passages
                .Include(p => p.TrafficViolations)
                .FirstOrDefaultAsync(p => p.GateId == 10);

            savedPassage.Should().NotBeNull();
            savedPassage!.TrafficViolations.Should().BeEmpty();
        }

        [Fact]
        public async Task PassageProcessingAsync_MissingVignette_CreatesNoVignetteViolation()
        {
            _context.TollGates.Add(
                new TollGate 
                { 
                    GateId = 20,
                    HighwayName = "D1",
                    Direction = "Praha",

                });



            _context.Vehicles.Add(
                new Vehicle 
                { 
                    VehicleId = 20, 
                    Spz = "5B97874", 
                    Type = VehicleType.Car
                });


            await _context.SaveChangesAsync();

            var dto = new RegisterTollPassDto
            {
                TollGateId = 20,
                DetectedSpz = "5B97874",
                VehicleSpeed = 100
            };




            _mockVignetteService.Setup(s => s.CheckVignetteAsync(It.IsAny<Vehicle>(), It.IsAny<DateTime>())).ReturnsAsync(false);
            _mockVehicleInspectionService.Setup(s => s.IsInspectionAndEmissionValidAsync(It.IsAny<Vehicle>(), It.IsAny<DateTime>())).ReturnsAsync((true, true));
            _mockSpeedService.Setup(s => s.GetSpeedOverLimit(It.IsAny<Passage>(), It.IsAny<Vehicle>())).Returns((int?)null);

            var res = await _sut.PassageProcessingAsync(dto);


            res.Should().NotBeNull();
            res!.HasViolations.Should().BeTrue();



            var savedPassage = await _context.Passages.Include(p => p.TrafficViolations).FirstOrDefaultAsync(p => p.GateId == 20);

            savedPassage.Should().NotBeNull();
            savedPassage!.TrafficViolations.Should().HaveCount(1);
            savedPassage.TrafficViolations.First().ViolationType.Should().Be(ViolationTypeCode.NoVignette);
            savedPassage.TrafficViolations.First().ActualPenaltyAmount.Should().Be(1500.0m);
        }



        [Fact]
        public async Task PassageProcessingAsync_ExpiredInspectionAndEmissions_CreatesTwoViolations()
        {
            _context.TollGates.Add(new TollGate
            {
                GateId = 30,
                HighwayName = "D1",
                Direction = "Praha",
            });
            _context.Vehicles.Add(
                new Vehicle
                {
                    VehicleId = 30, 
                    Spz = "3J56112",
                    Type = VehicleType.Car
                });


            await _context.SaveChangesAsync();

            var dto = new RegisterTollPassDto
            {
                TollGateId = 30,
                DetectedSpz = "3J56112",
                VehicleSpeed = 100
            };



            _mockVignetteService.Setup(s => s.CheckVignetteAsync(It.IsAny<Vehicle>(), It.IsAny<DateTime>())).ReturnsAsync(true);
            _mockVehicleInspectionService.Setup(s => s.IsInspectionAndEmissionValidAsync(It.IsAny<Vehicle>(), It.IsAny<DateTime>())).ReturnsAsync((false, false));
            _mockSpeedService.Setup(s => s.GetSpeedOverLimit(It.IsAny<Passage>(), It.IsAny<Vehicle>())).Returns((int?)null);

            var res = await _sut.PassageProcessingAsync(dto);




            res.Should().NotBeNull();
            res!.HasViolations.Should().BeTrue();

            var savedPassage = await _context.Passages.Include(p => p.TrafficViolations).FirstOrDefaultAsync(p => p.GateId == 30);
            savedPassage.Should().NotBeNull();
            savedPassage!.TrafficViolations.Should().HaveCount(2);
            savedPassage.TrafficViolations.Should().Contain(v => v.ViolationType == ViolationTypeCode.ExpiredVehicleInspection && v.ActualPenaltyAmount == 2000.0m);
            savedPassage.TrafficViolations.Should().Contain(v => v.ViolationType == ViolationTypeCode.ExpiredEmission && v.ActualPenaltyAmount == 1500.0m);
        }




        [Theory]
        [InlineData(101, 201, "6J66666", 10, 1000.0)]
        [InlineData(102, 202, "6J66667", 30, 2500.0)]
        [InlineData(103, 203, "6J66668", 60, 5000.0)]
        public async Task PassageProcessingAsync_SpeedingTiers_CreatesCorrectPenalty(int gateId, long vehicleId, string spz, int speedOver, decimal expectedPenalty)
        {
            _context.TollGates.Add(new TollGate
            {
                GateId = gateId,
                HighwayName = "D1",
                Direction = "Praha",
            });
            _context.Vehicles.Add(new Vehicle
            {
                VehicleId = vehicleId,
                Spz = spz,
                Type = VehicleType.Truck
            });
            await _context.SaveChangesAsync();

            var dto = new RegisterTollPassDto
            {
                TollGateId = gateId,
                DetectedSpz = spz,
                VehicleSpeed = 90 + speedOver
            };

            _mockVignetteService.Setup(s => s.CheckVignetteAsync(It.IsAny<Vehicle>(), It.IsAny<DateTime>())).ReturnsAsync(true);
            _mockVehicleInspectionService.Setup(s => s.IsInspectionAndEmissionValidAsync(It.IsAny<Vehicle>(), It.IsAny<DateTime>())).ReturnsAsync((true, true));
            _mockSpeedService.Setup(s => s.GetSpeedOverLimit(It.IsAny<Passage>(), It.IsAny<Vehicle>())).Returns(speedOver);



            var res = await _sut.PassageProcessingAsync(dto);
            res.Should().NotBeNull();
            res!.HasViolations.Should().BeTrue();

            var savedPassage = await _context.Passages.Include(p => p.TrafficViolations).FirstOrDefaultAsync(p => p.GateId == gateId);
            savedPassage.Should().NotBeNull();
            savedPassage!.TrafficViolations.Should().HaveCount(1);
            savedPassage.TrafficViolations.First().ViolationType.Should().Be(ViolationTypeCode.Speeding);
            savedPassage.TrafficViolations.First().ActualPenaltyAmount.Should().Be(expectedPenalty);
        }


        
    }
}
