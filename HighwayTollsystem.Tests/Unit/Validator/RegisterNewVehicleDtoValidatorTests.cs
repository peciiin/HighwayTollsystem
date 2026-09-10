using FluentValidation.TestHelper;
using HighwayTollsystem.DTOs;
using HighwayTollsystem.Enums;
using HighwayTollsystem.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HighwayTollsystem.Tests.Unit.Validator
{
    public class RegisterNewVehicleDtoValidatorTests
    {
        private readonly RegisterNewVehicleDtoValidator _validator;

        public RegisterNewVehicleDtoValidatorTests()
        {
            _validator = new RegisterNewVehicleDtoValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Spz_WhenEmptyOrNull_ShouldHaveValidationError(string? spz)
        {
            var model = new RegisterNewVehicleDto
            {
                Spz = spz!
            };
            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.Spz)
                  .WithErrorMessage("License plate is required.");
        }

        [Theory]
        [InlineData("AB")]
        [InlineData("ABCDEFGHIJK")]
        public void Spz_WhenLengthNotBetween3And10_ShouldHaveValidationError(string spz)
        {
            var model = new RegisterNewVehicleDto
            {
                Spz = spz
            };
            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.Spz)
                  .WithErrorMessage("License plate must be between 3 and 10 characters.");
        }

        [Theory]
        [InlineData("ABC@123")]
        [InlineData("ABC#123")]
        public void Spz_WhenContainsInvalidCharacters_ShouldHaveValidationError(string spz)
        {
            var model = new RegisterNewVehicleDto
            {
                Spz = spz
            };
            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.Spz)
                  .WithErrorMessage("License plate contains invalid characters.");
        }

        [Theory]
        [InlineData(999)]
        [InlineData(-1)]
        public void Type_WhenValueNotInEnum_ShouldHaveValidationError(int invalidTypeValue)
        {
            var model = new RegisterNewVehicleDto
            {
                Type = (VehicleType)invalidTypeValue
            };

            var res = _validator.TestValidate(model);

            res.ShouldHaveValidationErrorFor(x => x.Type)
               .WithErrorMessage("Invalid vehicle type.");
        }

        [Theory]
        [InlineData(999)]
        [InlineData(-1)]
        public void FuelType_WhenValueNotInEnum_ShouldHaveValidationError(int invalidFuelValue)
        {
            var model = new RegisterNewVehicleDto
            {
                FuelType = (FuelType)invalidFuelValue
            };

            var res = _validator.TestValidate(model);

            res.ShouldHaveValidationErrorFor(x => x.FuelType)
               .WithErrorMessage("Invalid fuel type.");
        }

        [Theory]
        [InlineData(999)]
        [InlineData(-1)]
        public void EmissionClass_WhenValueNotInEnum_ShouldHaveValidationError(int invalidEmissionValue)
        {
            var model = new RegisterNewVehicleDto
            {
                EmissionClass = (EmissionClass)invalidEmissionValue
            };

            var res = _validator.TestValidate(model);

            res.ShouldHaveValidationErrorFor(x => x.EmissionClass)
               .WithErrorMessage("Invalid emission class.");
        }

        [Fact]
        public void Enums_WhenValid_ShouldNotHaveValidationError()
        {
            var model = new RegisterNewVehicleDto
            {
                Type = VehicleType.Truck,
                FuelType = FuelType.Diesel,
                EmissionClass = EmissionClass.Euro3
            };
            var res = _validator.TestValidate(model);
            res.ShouldNotHaveValidationErrorFor(x => x.Type);
            res.ShouldNotHaveValidationErrorFor(x => x.FuelType);
            res.ShouldNotHaveValidationErrorFor(x => x.EmissionClass);
        }


        [Theory]
        [InlineData("US")]
        [InlineData("DE")]
        public void CountryCode_WhenValid_ShouldNotHaveValidationError(string countryCode)
        {
            var model = new RegisterNewVehicleDto
            {
                CountryCode = countryCode
            };
            var res = _validator.TestValidate(model);
            res.ShouldNotHaveValidationErrorFor(x => x.CountryCode);
        }

        [Theory]
        [InlineData("U")]
        [InlineData("USA")]
        public void CountryCode_WhenNotTwoLetters_ShouldHaveValidationError(string countryCode)
        {
            var model = new RegisterNewVehicleDto
            {
                CountryCode = countryCode
            };
            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.CountryCode)
                  .WithErrorMessage("Country code must be a 2-letter ISO code.");
        }
        [Theory]
        [InlineData("1HGCM82633A004352")]
        [InlineData("JH4KA8260MC000000")]
        public void Vin_WhenValid_ShouldNotHaveValidationError(string vin)
        {
            var model = new RegisterNewVehicleDto
            {
                Vin = vin
            };
            var res = _validator.TestValidate(model);
            res.ShouldNotHaveValidationErrorFor(x => x.Vin);


        }
        [Theory]
        [InlineData("1HGCM82633A00435")]//16
        [InlineData("1HGCM82633A0043523")]//18
        public void Vin_WhenNot17Characters_ShouldHaveValidationError(string vin)
        {
            var model = new RegisterNewVehicleDto
            {
                Vin = vin
            };
            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.Vin)
                  .WithErrorMessage("VIN filter must be 17 alphanumeric characters long (excluding I, O, and Q).");
        }

        [Theory]
        [InlineData("1HGCM82633A00435I")]//I
        [InlineData("1HGCM82633A00435O")]//O
        [InlineData("1HGCM82633A00435Q")]//Q
        public void Vin_WhenContainsInvalidCharacters_ShouldHaveValidationError(string vin)
        {
            var model = new RegisterNewVehicleDto
            {
                Vin = vin
            };
            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.Vin)
                  .WithErrorMessage("VIN filter must be 17 alphanumeric characters long (excluding I, O, and Q).");
        }

    }
}
