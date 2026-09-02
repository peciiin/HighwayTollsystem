using FluentAssertions;
using HighwayTollsystem.Data;
using HighwayTollsystem.DTOs;
using HighwayTollsystem.Enums;
using HighwayTollsystem.Models;
using HighwayTollsystem.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;

namespace HighwayTollsystem.Tests.Integration
{
    public class AnalyticsControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public AnalyticsControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        public async Task ResetDbAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HighwayTollsystem.Models.HighwayTollContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var vehicle1 = new Vehicle
            {
                VehicleId = 1,
                Spz = "1B12222",
                CountryCode = "CZ",
                Vin = "AAAAAAAAAAAAAAAAA",
                Type = VehicleType.Car,
                FuelType = FuelType.Petrol,
                EmissionClass = EmissionClass.Euro6,
                RegisteredAt = DateTime.UtcNow.AddYears(-2)
            };

            var vehicle2 = new Vehicle
            {
                VehicleId = 2,
                Spz = "1B13333",
                CountryCode = "CZ",
                Vin = "BBBBBBBBBBBBBBBBB",
                Type = VehicleType.Car,
                FuelType = FuelType.Diesel,
                EmissionClass = EmissionClass.Euro6,
                RegisteredAt = DateTime.UtcNow.AddYears(-1)
            };



            var gate1 = new TollGate
            { 
                GateId = 1,
                HighwayName = "D1",
                KilometerPost = 10.0m,
                Direction = "Praha",
                GpsLatitude = 48.1486m,
                GpsLongitude = 17.1077m
            };

            var gate2 = new TollGate
            {
                GateId = 2,
                HighwayName = "D1",
                KilometerPost = 10.0m,
                Direction = "Brno",
                GpsLatitude = 48.1486m,
                GpsLongitude = 17.1077m
            };

            
            var passage1 = new Passage
            {
                PassageId = 1,
                VehicleId = vehicle1.VehicleId,
                GateId = gate1.GateId,
                Timestamp = DateTime.UtcNow.AddDays(-10),
                VehicleSpeed = 140,
                CalculatedFee = 0.0m,

            };

            var passage2 = new Passage
            {
                PassageId = 2,
                VehicleId = vehicle2.VehicleId,
                GateId = gate2.GateId,
                Timestamp = DateTime.UtcNow.AddDays(-5),
                VehicleSpeed = 80,
                CalculatedFee = 0.0m,
            };

            var violation1 = new TrafficViolation
            {
                ViolationId = 1,
                PassageId = passage1.PassageId,
                ViolationType = ViolationTypeCode.Speeding,
                Details = $"Limit: 130, speed recorded: {10}",
                ActualPenaltyAmount = 1000.0m


            };

            context.Vehicles.AddRange(vehicle1, vehicle2);
            context.TollGates.AddRange(gate1, gate2);
            context.Passages.AddRange(passage1, passage2);
            context.TrafficViolations.Add(violation1);


            await context.SaveChangesAsync();
        }


        [Fact]
        public async Task GetDashboard_ReturnsOkAndCorrectData()
        {
            await ResetDbAsync();

            var res = await _client.GetAsync("/api/analytics/dashboard");

            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var dashboard = await res.Content.ReadFromJsonAsync<AnalyticsDashboardDto>();
            dashboard.Should().NotBeNull();

        }

        [Fact]
        public async Task GetTopGates_ReturnsOkAndCorrectData()
        {
            await ResetDbAsync();
            var res = await _client.GetAsync("/api/analytics/top-gates?count=5");
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var topGates = await res.Content.ReadFromJsonAsync<List<AnalyticsGateStatsDto>>();
            topGates.Should().NotBeNull();
            topGates.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetViolationBreakdownReturnsOkAndCorrectData()
        {
            await ResetDbAsync();
            var res = await _client.GetAsync("/api/analytics/breakdown-violations");
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var breakdown = await res.Content.ReadFromJsonAsync<List<AnalyticsViolationsStatsDto>>();
            breakdown.Should().NotBeNull();
            breakdown.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetTopGates_InvalidCountReturnsBadRequest()
        {
            var response = await _client.GetAsync("/api/analytics/top-gates?count=0");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}


