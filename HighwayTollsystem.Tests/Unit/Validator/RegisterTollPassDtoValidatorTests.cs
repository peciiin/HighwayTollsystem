using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Xunit;
using HighwayTollsystem.DTOs;
using HighwayTollsystem.Validators;



namespace HighwayTollsystem.Tests.Unit.Validator
{
    public class RegisterTollPassDtoValidatorTests
    {
        private readonly RegisterTollPassDtoValidator _validator;

        public RegisterTollPassDtoValidatorTests()
        {
            _validator = new RegisterTollPassDtoValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void TollGateId_WhenZeroOrNegative_ShouldHaveValidationError(int gateId)
        {
            var model = new RegisterTollPassDto 
            {
                TollGateId = gateId 
            };

            var res = _validator.TestValidate(model);

            res.ShouldHaveValidationErrorFor(x => x.TollGateId)
                  .WithErrorMessage("TollGateId must be greater than 0.");
        }

        [Fact]
        public void TollGateId_WhenGreaterThanZero_ShouldNotHaveValidationError()
        {
            var model = new RegisterTollPassDto 
            {
                TollGateId = 1 
            };

            var res = _validator.TestValidate(model);

            res.ShouldNotHaveValidationErrorFor(x => x.TollGateId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void DetectedSpz_WhenEmptyOrNull_ShouldHaveValidationError(string? spz)
        {
            var model = new RegisterTollPassDto 
            { 
                DetectedSpz = spz! 
            };
            var res = _validator.TestValidate(model);

            res.ShouldHaveValidationErrorFor(x => x.DetectedSpz)
                  .WithErrorMessage("Spz is required.");
        }

        [Theory]
        [InlineData("A")]
        [InlineData("AB")]
        [InlineData("12345678901")]
        public void DetectedSpz_WhenLengthOutOfRange_ShouldHaveValidationError(string spz)
        {
            var model = new RegisterTollPassDto 
            { 
                DetectedSpz = spz 
            };

            var res = _validator.TestValidate(model);

            res.ShouldHaveValidationErrorFor(x => x.DetectedSpz)
                  .WithErrorMessage("Spz cannot exceed 10 characters.");
        }

        [Fact]
        public void DetectedSpz_WhenValid_ShouldNotHaveValidationError()
        {
            var model = new RegisterTollPassDto 
            { 
                DetectedSpz = "ABC123" 
            };
            var res = _validator.TestValidate(model);
            res.ShouldNotHaveValidationErrorFor(x => x.DetectedSpz);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(301)]
        public void VehicleSpeed_WhenOutOfRange_ShouldHaveValidationError(int speed)
        {
            var model = new RegisterTollPassDto 
            { 
                VehicleSpeed = speed
            };
            var res = _validator.TestValidate(model);
            res.ShouldHaveValidationErrorFor(x => x.VehicleSpeed)
                  .WithErrorMessage("Vehicle speed must be between 0 and 300.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(150)]
        [InlineData(300)]
        public void VehicleSpeed_WhenInRange_ShouldNotHaveValidationError(int speed)
        {
            var model = new RegisterTollPassDto 
            { 
                VehicleSpeed = speed 
            };
            var res = _validator.TestValidate(model);
            res.ShouldNotHaveValidationErrorFor(x => x.VehicleSpeed);
        }
    }
}
