using HighwayTollsystem.Interfaces;
using HighwayTollsystem.Models;
using HighwayTollsystem.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace HighwayTollsystem.Services
{
    public class TollSimulatorService : BackgroundService
    {
        
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TollSimulatorService> _logger;
        private readonly Random _random = new Random();

        private readonly string[] _sampleSpz = {"1A11111", "2B19876", "5J1245", "3J54721", "4B67890", "6E98765", "7U45678", "8U23456", "9B34567"};
        

        public TollSimulatorService(IServiceScopeFactory serviceScopeFactory, ILogger<TollSimulatorService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    var tollService = scope.ServiceProvider.GetRequiredService<ITollService>();
                    var dbContext = scope.ServiceProvider.GetRequiredService<HighwayTollContext>();

                    var idsOfGates = await dbContext.TollGates.Select(g => g.GateId).ToListAsync(stoppingToken);
                    if (idsOfGates.Count == 0)
                    {
                        _logger.LogWarning("No toll gates found in the database.");
                        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
                        continue;
                    }
                    var selectedGateid = idsOfGates[_random.Next(idsOfGates.Count)];
                    var plates = await dbContext.Vehicles.Select(v => v.Spz).Take(100).ToListAsync(stoppingToken);
                    string selectedSpz;

                    if (plates.Count > 0 && _random.Next(100) > 5) selectedSpz = plates[_random.Next(plates.Count)];
                    else selectedSpz = _sampleSpz[_random.Next(_sampleSpz.Length)];

                    var vehicleSpeed = _random.Next(80, 160);

                    var tollPassDto = new DTOs.RegisterTollPassDto
                    {
                        TollGateId = selectedGateid,
                        DetectedSpz = selectedSpz,
                        VehicleSpeed = vehicleSpeed
                    };
                    var result = await tollService.PassageProcessingAsync(tollPassDto);

                    if(result == null)
                    {
                        _logger.LogWarning("Passage processing returned null for GateId={GateId}, SPZ={SPZ}, Speed={Speed}",
                            selectedGateid, selectedSpz, vehicleSpeed);
                        continue;
                    }

                    _logger.LogInformation("Simulated passage: GateId={GateId}, SPZ={SPZ}, Speed={Speed}, Fee={Fee}, Violations={Violations}",
                        result.GateId, result.DetectedSpz, result.VehicleSpeed, result.CalculatedFee, result.HasViolations);

                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "Error during simulation");
                }
                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            }
        }
    


    }
}
