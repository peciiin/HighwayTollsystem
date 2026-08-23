using FluentValidation;
using HighwayTollsystem.DTOs;
namespace HighwayTollsystem.Validators
{
    public class RegisterTollPassDtoValidator : AbstractValidator<RegisterTollPassDto>
    {
        public RegisterTollPassDtoValidator()
        {
            RuleFor(x => x.TollGateId)
                .GreaterThan(0).WithMessage("TollGateId must be greater than 0.");
            RuleFor(x => x.DetectedSpz)
                .NotEmpty().WithMessage("DetectedSpz is required.")
                .Length(3,10).WithMessage("DetectedSpz cannot exceed 10 characters.");
            RuleFor(x => x.VehicleSpeed)
                .InclusiveBetween(0, 300).WithMessage("VehicleSpeed must be between 0 and 300.");
        }
    }
}
