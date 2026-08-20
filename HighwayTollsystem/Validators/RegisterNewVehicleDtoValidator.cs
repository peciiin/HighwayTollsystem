using FluentValidation;
using HighwayTollsystem.DTOs;
namespace HighwayTollsystem.Validators
{
    public class RegisterNewVehicleDtoValidator : AbstractValidator<RegisterNewVehicleDto>
    {
        public RegisterNewVehicleDtoValidator()
        {
            RuleFor(x => x.Spz)
                .NotEmpty().WithMessage("License plate is required.").Length(3, 10).WithMessage("License plate must be between 3 and 10 characters.")
                .Matches(@"^[a-zA-Z0-9 -]+$").WithMessage("License plate contains invalid characters.");
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid vehicle type.");
            RuleFor(x => x.FuelType)
                .IsInEnum().WithMessage("Invalid fuel type.");
            RuleFor(x => x.EmissionClass)
                .IsInEnum().WithMessage("Invalid emission class.");
            RuleFor(x => x.CountryCode)
                .Matches("^[a-zA-Z]{2}$").When(x => !string.IsNullOrEmpty(x.CountryCode))
                .WithMessage("Country code must be a 2-letter ISO code.");
            RuleFor(x => x.Vin)
                .Matches("^[A-HJ-NPR-Za-hj-npr-z0-9]{17}$").When(x => !string.IsNullOrEmpty(x.Vin))
                .WithMessage("VIN filter must be 17 alphanumeric characters long (excluding I, O, and Q).");

        }
    }
}
