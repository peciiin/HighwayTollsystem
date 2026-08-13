using HighwayTollsystem.Enums;
using HighwayTollsystem.Interfaces;
using HighwayTollsystem.Models;
using HighwayTollsystem.Services;
using Microsoft.EntityFrameworkCore;
namespace HighwayTollsystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(HighwayTollContext db, ITollService tollService)
        {
            await db.Database.EnsureCreatedAsync();


            if (!await db.TollGates.AnyAsync())
            {
                var tollGates = new List<TollGate>
                {
                    new TollGate { HighwayName = "D1", KilometerPost = 12.5m, Direction = "Brno", GpsLatitude = 49.9982m, GpsLongitude = 14.5931m },
                    new TollGate { HighwayName = "D1", KilometerPost = 12.5m, Direction = "Praha", GpsLatitude = 49.9982m, GpsLongitude = 14.5931m },
                    new TollGate { HighwayName = "D1", KilometerPost = 66.0m, Direction = "Brno", GpsLatitude = 49.6541m, GpsLongitude = 15.0874m },
                    new TollGate { HighwayName = "D1", KilometerPost = 66.0m, Direction = "Praha", GpsLatitude = 49.6541m, GpsLongitude = 15.0874m },
                    new TollGate { HighwayName = "D1", KilometerPost = 188.0m, Direction = "Brno", GpsLatitude = 49.1722m, GpsLongitude = 16.5311m },
                    new TollGate { HighwayName = "D1", KilometerPost = 188.0m, Direction = "Praha", GpsLatitude = 49.1722m, GpsLongitude = 16.5311m },
                    new TollGate { HighwayName = "D8", KilometerPost = 18.0m, Direction = "Ústí nad Labem", GpsLatitude = 50.2511m, GpsLongitude = 14.3120m },
                    new TollGate { HighwayName = "D8", KilometerPost = 18.0m, Direction = "Praha", GpsLatitude = 50.2511m, GpsLongitude = 14.3120m },
                    new TollGate { HighwayName = "D2", KilometerPost = 15.0m, Direction = "Bratislava", GpsLatitude = 49.0521m, GpsLongitude = 16.6852m },
                    new TollGate { HighwayName = "D2", KilometerPost = 15.0m, Direction = "Brno", GpsLatitude = 49.0521m, GpsLongitude = 16.6852m }
                };
                db.TollGates.AddRange(tollGates);
                await db.SaveChangesAsync();
            }





            if (await db.Vehicles.AnyAsync()) return;

            var vehicles = new List<Vehicle>();

            var vehicleTypes = Enum.GetValues<VehicleType>().ToList();
            var countryCodes = new List<string> { "CZ", "CZ", "CZ", "CZ", "CZ", "CZ", "CZ", "CZ", "CZ", "DE", "AT", "SK", "PL" };

            for (int i = 0; i < 100; i++)
            {
                var vehicleType = Random.Shared.Next(0, 100) < 80 ? VehicleType.Car : vehicleTypes[Random.Shared.Next(vehicleTypes.Count)];
                var registeredAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 365 * 26));
                var fuelType = GetRandomFuelType(vehicleType);
                var emissionClass = GetEmissionClassForYear(registeredAt.Year);
                var countryCode = countryCodes[Random.Shared.Next(countryCodes.Count)];
                var spz = GenerateSpzForCountry(countryCode);


                vehicles.Add(new Vehicle
                {
                    Spz = spz,
                    CountryCode = countryCode,
                    Vin = GenerateVin(),
                    Type = vehicleType,
                    FuelType = fuelType,
                    EmissionClass = emissionClass,
                    RegisteredAt = registeredAt
                });
            }

            db.Vehicles.AddRange(vehicles);
            await db.SaveChangesAsync();


            
            


            if (!await db.Vignettes.AnyAsync())
            {
                var vignettes = new List<Vignette>();
                foreach (var vehicle in vehicles)
                {
                    if (Random.Shared.Next(0, 100) < 90)
                    {
                        bool isValid = Random.Shared.Next(0, 100) < 85;
                        var validFrom = isValid
                            ? DateTime.UtcNow.AddDays(-Random.Shared.Next(10, 200))
                            : DateTime.UtcNow.AddDays(-Random.Shared.Next(400, 600));

                        vignettes.Add(new Vignette
                        {
                            VehicleId = vehicle.VehicleId,
                            PurchaseDate = validFrom.AddDays(-1),
                            ValidFrom = validFrom,
                            ValidTo = validFrom.AddYears(1)
                        });
                    }
                }
                db.Vignettes.AddRange(vignettes);
                await db.SaveChangesAsync();
            }


            if (!await db.VehicleInspections.AnyAsync())
            {
                var inspections = new List<VehicleInspection>();
                foreach (var vehicle in vehicles)
                {
                    DateTime stkValidTo;
                    DateTime emissionsValidTo;
                    if (Random.Shared.Next(0, 100) < 95)
                    {
                        stkValidTo = DateTime.UtcNow.AddDays(Random.Shared.Next(30, 730));
                        emissionsValidTo = stkValidTo;
                    }
                    else
                    {
                        int problemType = Random.Shared.Next(0, 3);
                        if (problemType == 0)
                        {
                            stkValidTo = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 180));
                            emissionsValidTo = DateTime.UtcNow.AddDays(Random.Shared.Next(30, 365));
                        }
                        else if (problemType == 1)
                        {
                            stkValidTo = DateTime.UtcNow.AddDays(Random.Shared.Next(30, 365));
                            emissionsValidTo = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 180));
                        }
                        else
                        {
                            stkValidTo = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 180));
                            emissionsValidTo = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 180));
                        }
                    }

                    inspections.Add(new VehicleInspection
                    {
                        VehicleId = vehicle.VehicleId,
                        CreatedAt = DateTime.UtcNow.AddYears(-2),
                        ValidTo = stkValidTo,
                        EmissionsValidTo = emissionsValidTo
                    });
                }
                db.VehicleInspections.AddRange(inspections);
                await db.SaveChangesAsync();
            }




            if (!await db.Passages.AnyAsync())
            {
                var tollGates = await db.TollGates.ToListAsync();
                var existingVehicles = await db.Vehicles.ToListAsync();

                // 1. Generování průjezdů pro REGISTROVANÁ vozidla
                foreach (var vehicle in existingVehicles)
                {
                    int passageCount = Random.Shared.Next(1, 15);
                    for (int j = 0; j < passageCount; j++)
                    {
                        var tollGate = tollGates[Random.Shared.Next(tollGates.Count)];

                        int speed = Random.Shared.Next(0, 100) < 85
                            ? Random.Shared.Next(80, 130)
                            : Random.Shared.Next(135, 180);

                        var passage = new Passage
                        {
                            GateId = tollGate.GateId,
                            Timestamp = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 365 * 2)),
                            VehicleSpeed = speed
                        };

                        await tollService.PassageProcessingAsync(passage, vehicle.Spz);
                    }
                }

                for (int k = 0; k < 10; k++)
                {
                    var tollGate = tollGates[Random.Shared.Next(tollGates.Count)];
                    var passage = new Passage
                    {
                        GateId = tollGate.GateId,
                        Timestamp = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 30)),
                        VehicleSpeed = 110
                    };

                    await tollService.PassageProcessingAsync(passage, "UNKNOWN");
                }
            }
        }

        private static string GenerateSpzForCountry(string countryCode)
        {
            return countryCode switch
            {
                "CZ" => $"{Random.Shared.Next(1, 10)}{(char)Random.Shared.Next('A', 'Z' + 1)}{Random.Shared.Next(0, 10)} {Random.Shared.Next(1000, 10000)}",
                "SK" => $"{(char)Random.Shared.Next('A', 'Z' + 1)}{(char)Random.Shared.Next('A', 'Z' + 1)}-{Random.Shared.Next(100, 1000)}{(char)Random.Shared.Next('A', 'Z' + 1)}{(char)Random.Shared.Next('A', 'Z' + 1)}",
                "DE" => $"{(char)Random.Shared.Next('A', 'Z' + 1)}{(char)Random.Shared.Next('A', 'Z' + 1)}-{(char)Random.Shared.Next('A', 'Z' + 1)}{(char)Random.Shared.Next('A', 'Z' + 1)} {Random.Shared.Next(100, 10000)}",
                _ => $"{Random.Shared.Next(100, 999)}-{(char)Random.Shared.Next('A', 'Z' + 1)}{(char)Random.Shared.Next('A', 'Z' + 1)}"
            };
        }



        private static string GenerateVin()
        {
            const string chars = "ABCDEFGHJKLMNPRSTUVWXYZ0123456789";
            char[] vin = new char[17];
            for (int i = 0; i < 17; i++) vin[i] = chars[Random.Shared.Next(chars.Length)];
            return new string(vin);
        }


        private static FuelType GetRandomFuelType(VehicleType vehicleType)
        {
            return vehicleType switch
            {
                VehicleType.Motorcycle => Random.Shared.Next(0, 10) < 9 ? FuelType.Petrol : FuelType.Electric,
                VehicleType.Truck => Random.Shared.Next(0, 10) < 9 ? FuelType.Diesel : FuelType.Electric,
                VehicleType.Car => (FuelType)Random.Shared.Next(0, Enum.GetValues<FuelType>().Length),
                _ => Random.Shared.Next(0, 10) < 8 ? FuelType.Diesel : FuelType.Petrol
            };
        }



        private static EmissionClass GetEmissionClassForYear(int year)
        {
            if (year >= 2015) return EmissionClass.Euro6;
            if (year >= 2011) return EmissionClass.Euro5;
            if (year >= 2006) return EmissionClass.Euro4;

            var olderClasses = new[] { EmissionClass.Euro1, EmissionClass.Euro2, EmissionClass.Euro3 };
            return olderClasses[Random.Shared.Next(olderClasses.Length)];
        }
    }
}