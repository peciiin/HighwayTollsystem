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
            var model = new RegisterTollPassDto { TollGateId = gateId };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.TollGateId)
                  .WithErrorMessage("TollGateId must be greater than 0.");
        }

        [Fact]
        public void TollGateId_WhenGreaterThanZero_ShouldNotHaveValidationError()
        {
            var model = new RegisterTollPassDto { TollGateId = 1 };

            var result = _validator.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x => x.TollGateId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void DetectedSpz_WhenEmptyOrNull_ShouldHaveValidationError(string? spz)
        {
            var model = new RegisterTollPassDto { DetectedSpz = spz! };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.DetectedSpz)
                  .WithErrorMessage("Spz is required.");
        }

        [Theory]
        [InlineData("A")]
        [InlineData("AB")]
        [InlineData("12345678901")]
        public void DetectedSpz_WhenLengthOutOfRange_ShouldHaveValidationError(string spz)
        {
            var model = new RegisterTollPassDto { DetectedSpz = spz };

            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.DetectedSpz)
                  .WithErrorMessage("Spz cannot exceed 10 characters.");
        }
    }
}
