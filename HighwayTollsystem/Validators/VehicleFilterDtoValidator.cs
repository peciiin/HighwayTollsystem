using FluentValidation;
using HighwayTollsystem.DTOs;



namespace HighwayTollsystem.Validators
{
    public class VehicleFilterDtoValidator : AbstractValidator<VehicleFilterDto>
    {
        public VehicleFilterDtoValidator() 
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
            RuleFor(x => x.FuelType)
                .IsInEnum().When(x => x.FuelType.HasValue).WithMessage("Invalid fuel type.");
            RuleFor(x => x.VehicleType)
                .IsInEnum().When(x => x.VehicleType.HasValue).WithMessage("Invalid vehicle type.");
            RuleFor(x => x.EmissionClass)
                .IsInEnum().When(x => x.EmissionClass.HasValue).WithMessage("Invalid emission class.");
            RuleFor(x => x.CountryCode)
                .Matches("^[a-zA-Z]{2}$").When(x => !string.IsNullOrEmpty(x.CountryCode))
                .WithMessage("Country code must be a 2-letter ISO code.");
            RuleFor(x => x.Vin)
                .Matches("^[A-HJ-NPR-Za-hj-npr-z0-9]{1,17}$").When(x => !string.IsNullOrEmpty(x.Vin))
                .WithMessage("VIN filter must be up to 17 alphanumeric characters (excluding I, O, and Q).");
            RuleFor(x => x.Spz)
                .Matches("^[a-zA-Z0-9 -]{1,10}$").When(x => !string.IsNullOrEmpty(x.Spz))
                .WithMessage("SPZ must be an alphanumeric string with a maximum length of 10.");
            RuleFor(x => x.RegisteredFrom)
                .LessThanOrEqualTo(x => x.RegisteredTo!.Value)
                .When(x => x.RegisteredFrom.HasValue && x.RegisteredTo.HasValue)
                .WithMessage("RegisteredFrom must be less than or equal to RegisteredTo.");
        }
    }
}
