using HighwayTollsystem.Enums;
using HighwayTollsystem.Interfaces;
using HighwayTollsystem.Models;
using HighwayTollsystem.Services;
using Microsoft.EntityFrameworkCore;

namespace HighwayTollsystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(HighwayTollContext db)
        {
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

                var emissionClass = fuelType == FuelType.Electric ? EmissionClass.EV : GetEmissionClassForYear(registeredAt.Year);

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




            var vignettes = new List<Vignette>();
            if (!await db.Vignettes.AnyAsync())
            {
                foreach (var vehicle in vehicles)
                {
                    if (vehicle.EmissionClass == EmissionClass.EV || vehicle.Type == VehicleType.Motorcycle || vehicle.Type == VehicleType.Truck || vehicle.Type == VehicleType.Other)
                    {
                        continue;
                    }



                    if (Random.Shared.Next(0, 100) < 90)
                    {
                        bool isValid = Random.Shared.Next(0, 100) < 85;
                        var validFrom = isValid ? DateTime.UtcNow.AddDays(-Random.Shared.Next(100, 180))
                            : DateTime.UtcNow.AddDays(-Random.Shared.Next(500, 650));

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

            var inspections = new List<VehicleInspection>();
            if (!await db.VehicleInspections.AnyAsync())
            {



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
                            stkValidTo = DateTime.UtcNow.AddDays(-Random.Shared.Next(100, 250));
                            emissionsValidTo = DateTime.UtcNow.AddDays(Random.Shared.Next(30, 365));
                        }
                        else if (problemType == 1)
                        {
                            stkValidTo = DateTime.UtcNow.AddDays(Random.Shared.Next(30, 365));
                            emissionsValidTo = DateTime.UtcNow.AddDays(-Random.Shared.Next(100, 250));
                        }
                        else
                        {
                            stkValidTo = DateTime.UtcNow.AddDays(-Random.Shared.Next(100, 250));
                            emissionsValidTo = DateTime.UtcNow.AddDays(-Random.Shared.Next(100, 250));
                        }
                    }

                    inspections.Add(new VehicleInspection
                    {
                        VehicleId = vehicle.VehicleId,
                        CreatedAt = DateTime.UtcNow.AddYears(-2),
                        ValidTo = stkValidTo,
                        EmissionsValidTo = vehicle.EmissionClass == EmissionClass.EV ? DateTime.UtcNow.AddYears(10) : emissionsValidTo
                    });
                }
                db.VehicleInspections.AddRange(inspections);
                await db.SaveChangesAsync();
            }

            if (!await db.Passages.AnyAsync())
            {
                var tollGates = await db.TollGates.ToListAsync();
                var existingVehicles = await db.Vehicles.ToListAsync();

                var passages = new List<Passage>();

                var violations = new List<TrafficViolation>();

                foreach (var vehicle in existingVehicles)
                {



                    int passageCount = Random.Shared.Next(1, 15);
                    for (int j = 0; j < passageCount; j++)
                    {
                        var tollGate = tollGates[Random.Shared.Next(tollGates.Count)];
                        int speed = Random.Shared.Next(0, 100) < 85
                            ? Random.Shared.Next(80, 130)
                            : Random.Shared.Next(135, 180);

                        var timestamp = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 90)).AddHours(-Random.Shared.Next(0, 24));

                        var tollFee = vehicle.Type switch
                        {
                            VehicleType.Other => 100.0m,
                            VehicleType.Truck => 150.0m,
                            _ => 0.0m
                        };

                        var passage = new Passage
                        {
                            VehicleId = vehicle.VehicleId,
                            CalculatedFee = tollFee,
                            Timestamp = timestamp,
                            VehicleSpeed = speed,
                            GateId = tollGate.GateId,

                        };
                        passages.Add(passage);
                        




                        if (vehicle.Type != VehicleType.Truck && vehicle.Type != VehicleType.Motorcycle && vehicle.Type != VehicleType.Other && vehicle.EmissionClass != EmissionClass.EV)
                        {
                            bool hasVignette = vignettes.Any(v => v.VehicleId == vehicle.VehicleId && v.ValidFrom <= timestamp && v.ValidTo >= timestamp);


                            if (!hasVignette)
                            {
                                violations.Add(new TrafficViolation
                                {

                                    Passage = passage,
                                    ViolationType = ViolationTypeCode.NoVignette,
                                    Details = "Vehicle is missing valid vignette.",
                                    ActualPenaltyAmount = 1500.0m
                                });
                            }
                        }






                        int speedLimit = vehicle.Type == VehicleType.Truck ? 90 : 130;
                        double speedWithTolerance = speed > 100 ? speed * 0.97 : speed - 3;
                        int speedOver = (int)Math.Round(speedWithTolerance) - speedLimit;

                        if (speedOver > 0)
                        {
                            decimal penalty = speedOver switch
                            {
                                < 20 => 1000.0m,
                                < 50 => 2500.0m,
                                _ => 5000.0m
                            };

                            violations.Add(new TrafficViolation
                            {
                                Passage = passage,
                                ViolationType = ViolationTypeCode.Speeding,
                                Details = $"Max speed for {vehicle.Type}: {speedLimit} km/h. Detected: {speed} km/h. Exceeded by + {speedOver} km/h.",
                                ActualPenaltyAmount = penalty
                            });
                        }






                        var inspection = inspections.FirstOrDefault(i => i.VehicleId == vehicle.VehicleId);
                        if (inspection != null)
                        {
                            if (inspection.ValidTo < timestamp)
                            {
                                violations.Add(new TrafficViolation
                                {
                                    Passage = passage,
                                    ViolationType = ViolationTypeCode.ExpiredVehicleInspection,
                                    Details = "Vehicle has expired technical inspection.",
                                    ActualPenaltyAmount = 2000.0m
                                });
                            }




                            if (vehicle.EmissionClass != EmissionClass.EV && inspection.EmissionsValidTo < timestamp)
                            {
                                violations.Add(new TrafficViolation
                                {
                                    Passage = passage,
                                    ViolationType = ViolationTypeCode.ExpiredEmission,
                                    Details = "Vehicle has expired emissions.",
                                    ActualPenaltyAmount = 1500.0m
                                });
                            }
                        }
                    }
                }

                for (int k = 0; k < 10; k++)
                {
                    var tollGate = tollGates[Random.Shared.Next(tollGates.Count)];


                    var timestamp = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 30));

                    int speed = Random.Shared.Next(80, 180);

                    var unknown = new Passage
                    {
                        VehicleId = null,
                        CalculatedFee = 0.0m,
                        VehicleSpeed = speed,
                        Timestamp = timestamp,
                        GateId = tollGate.GateId
                    };


                    passages.Add(unknown);

                    violations.Add(new TrafficViolation
                    {
                        Passage = unknown,
                        ViolationType = ViolationTypeCode.NoVignette,
                        Details = "Vehicle is not registered in the system.",
                        ActualPenaltyAmount = 5000.0m
                    });
                }

                db.Passages.AddRange(passages);
                db.TrafficViolations.AddRange(violations);
                await db.SaveChangesAsync();
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