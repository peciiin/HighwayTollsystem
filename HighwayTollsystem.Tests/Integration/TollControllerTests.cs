using HighwayTollsystem.DTOs;
using HighwayTollsystem.Enums;
using HighwayTollsystem.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;



namespace HighwayTollsystem.Tests.Integration
{
    public class TollControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        public TollControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            
        }

        private async Task ResetDbAsync()
        {
            await _factory.ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HighwayTollContext>();


            await context.SaveChangesAsync();

            var gate = new TollGate
            {
                GateId = 1,
                HighwayName = "D1",
                KilometerPost = 15.5m,
                Direction = "Brno",
                GpsLatitude = 49.1951m,
                GpsLongitude = 16.6068m
            };

            var vehicle = new Vehicle
            {
                VehicleId = 1,
                Spz = "1A12222",
                CountryCode = "CZ",
                Vin = "11111111111111111",
                Type = VehicleType.Car,
                FuelType = FuelType.Petrol,
                EmissionClass = EmissionClass.Euro5,
                RegisteredAt = DateTime.UtcNow.AddMonths(-8)
            };

            context.TollGates.Add(gate);
            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();
        }

        [Fact]

        public async Task RegisterTollPass_ReturnsOk_WhenValidRequest()
        {
            await ResetDbAsync();

            var req = new RegisterTollPassDto
            {
                TollGateId = 1,
                DetectedSpz = "1A12222",
                VehicleSpeed = 80
            };

            var response = await _client.PostAsJsonAsync("/api/toll", req);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<PassageResponseDto>();
            result.Should().NotBeNull();
            result!.VehicleId.Should().Be(1);
            result.GateId.Should().Be(req.TollGateId);
            result.DetectedSpz.Should().Be(req.DetectedSpz);
            result.VehicleSpeed.Should().Be(req.VehicleSpeed);
            
        }

        [Fact]
        public async Task RegisterTollPass_ReturnsNotFound_WhenGateDoesNotExist()
        {
            await ResetDbAsync();

            var req = new RegisterTollPassDto
            {
                TollGateId = 3,
                DetectedSpz = "1A12222",
                VehicleSpeed = 80
            };

            var response = await _client.PostAsJsonAsync("/api/toll", req);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


    }
}
