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
    
    public class VehicleFilterDtoValidatorTests
    {
        private readonly VehicleFilterDtoValidator _validator;

        public VehicleFilterDtoValidatorTests()
        {
            _validator = new VehicleFilterDtoValidator();
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void PageNumber_WhenLessThanOrEqualToZero_ShouldHaveValidationError(int pageNumber)
        {
            var model = new VehicleFilterDto
            {
                PageNumber = pageNumber,
            };
            
            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.PageNumber)
                .WithErrorMessage("Page number must be greater than 0.");

        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        public void PageNumber_WhenGreaterThanZero_ShouldNotHaveValidationError(int pageNumber)
        {
            var model = new VehicleFilterDto
            {
                PageNumber = pageNumber,
            };
            var res = _validator.TestValidate(model);
            res.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
        }



        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void PageSize_WhenNotBetween1And100_ShouldHaveValidationError(int pageSize)
        {
            var model = new VehicleFilterDto
            {
                PageSize = pageSize,
            };
            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.PageSize)
                .WithErrorMessage("Page size must be between 1 and 100.");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public void PageSize_WhenBetween1And100_ShouldNotHaveValidationError(int pageSize)
        {
            var model = new VehicleFilterDto
            {
                PageSize = pageSize,
            };
            var res = _validator.TestValidate(model);
            res.ShouldNotHaveValidationErrorFor(x => x.PageSize);
        }

        [Theory]
        [InlineData(999)]
        [InlineData(-1)]
        public void VehicleType_WhenInvalid_ShouldHaveValidationError(int invalidEnumValue)
        {
            var model = new VehicleFilterDto
            {
                VehicleType = (VehicleType)invalidEnumValue,
            };
            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.VehicleType)
                .WithErrorMessage("Invalid vehicle type.");
            
        }

        [Theory]
        [InlineData(999)]
        [InlineData(-1)]
        public void FuelType_WhenInvalid_ShouldHaveValidationError(int invalidEnumValue)
        {
            var model = new VehicleFilterDto
            {
                FuelType = (FuelType)invalidEnumValue,
            };
            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.FuelType)
                .WithErrorMessage("Invalid fuel type.");
        }

        [Theory]
        [InlineData(999)]
        [InlineData(-1)]
        public void EmissionClass_WhenInvalid_ShouldHaveValidationError(int invalidEnumValue)
        {
            var model = new VehicleFilterDto
            {
                EmissionClass = (EmissionClass)invalidEnumValue,
            };
            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.EmissionClass)
                .WithErrorMessage("Invalid emission class.");
        }

        


        [Fact]
        public void Enums_WhenValid_ShouldNotHaveValidationError()
        {
            var model = new VehicleFilterDto
            {
                VehicleType = VehicleType.Motorcycle,
                FuelType = FuelType.Electric,
                EmissionClass = EmissionClass.EV
            };
            var res = _validator.TestValidate(model);
            res.ShouldNotHaveValidationErrorFor(x => x.VehicleType);
            res.ShouldNotHaveValidationErrorFor(x => x.FuelType);
            res.ShouldNotHaveValidationErrorFor(x => x.EmissionClass);
        }


        [Theory]
        [InlineData("US")]
        [InlineData("DE")]
        public void CountryCode_WhenValid_ShouldNotHaveValidationError(string countryCode)
        {
            var model = new VehicleFilterDto
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
            var model = new VehicleFilterDto
            {
                CountryCode = countryCode
            };

            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.CountryCode)
               .WithErrorMessage("Country code must be a 2-letter ISO code.");
        }

        [Fact]
        public void Vin_WhenLongerThan17Characters_ShouldHaveValidationError()
        {
            var model = new VehicleFilterDto 
            {
                Vin = "2AAAAAAAAAAA04352A"//18
            };

            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.Vin)
               .WithErrorMessage("VIN filter must be up to 17 alphanumeric characters (excluding I, O, and Q).");
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1AAABBB333A00435")]//16
        [InlineData("2AAAAAAAAAAA04352")]//17
        public void Vin_WhenValid_ShouldNotHaveValidationError(string vin)
        {
            var model = new VehicleFilterDto { Vin = vin };

            var res = _validator.TestValidate(model);
            res.ShouldNotHaveValidationErrorFor(x => x.Vin);
        }


        [Theory]
        [InlineData("AAAAAAAAAAAAAAAAI")]
        [InlineData("AAAAAAAAAAAAAAAAO")]
        [InlineData("AAAAAAAAAAAAAAAAQ")]
        public void Vin_WhenContainsInvalidCharacters_ShouldHaveValidationError(string vin)
        {
            var model = new VehicleFilterDto 
            { 
                Vin = vin
            };

            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.Vin)
               .WithErrorMessage("VIN filter must be up to 17 alphanumeric characters (excluding I, O, and Q).");
        }

        [Theory]
        [InlineData("1A12345")]
        [InlineData("ABC-12-34")]
        public void Spz_WhenValid_ShouldNotHaveValidationError(string spz)
        {
            var model = new VehicleFilterDto 
            { 
                Spz = spz 
            };

            var res = _validator.TestValidate(model);
            res.ShouldNotHaveValidationErrorFor(x => x.Spz);
        }

        [Theory]
        [InlineData("LONG1234567")]//11
        [InlineData("1A1@222")]//@
        public void Spz_WhenInvalid_ShouldHaveValidationError(string spz)
        {

            var model = new VehicleFilterDto 
            { 
                Spz = spz 
            };

            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.Spz)
               .WithErrorMessage("SPZ must be an alphanumeric string with a maximum length of 10.");
        }

        [Fact]
        public void RegistrationDates_WhenFromIsGreaterThanTo_ShouldHaveValidationError()
        {
            var model = new VehicleFilterDto
            {
                RegisteredFrom = DateTime.UtcNow.AddDays(100),
                RegisteredTo = DateTime.UtcNow
            };

            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.RegisteredFrom)
               .WithErrorMessage("RegisteredFrom must be less than or equal to RegisteredTo.");
        }

        [Fact]
        public void RegistrationDates_WhenFromIsLessThanOrEqualToTo_ShouldNotHaveValidationError()
        {
            var now = DateTime.UtcNow;
            var model = new VehicleFilterDto
            {
                RegisteredFrom = now.AddDays(-5),
                RegisteredTo = now
            };

            var res = _validator.TestValidate(model);
            res.ShouldNotHaveValidationErrorFor(x => x.RegisteredFrom);
        }


    }
}
