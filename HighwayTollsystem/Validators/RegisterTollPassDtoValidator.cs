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
                .NotEmpty().WithMessage("Spz is required.")
                .Length(3,10).WithMessage("Spz cannot exceed 10 characters.");
            RuleFor(x => x.VehicleSpeed)
                .InclusiveBetween(0, 300).WithMessage("Vehicle speed must be between 0 and 300.");
        }
    }
}
