using FluentValidation;
using PharmacyManagement.DTOs.Manufacturer;

namespace PharmacyManagement.Validators.FluentValidation.Manufacturer
{
    public class UpdateManufacturerValidator
        : AbstractValidator<UpdateManufacturerRequest>
    {
        public UpdateManufacturerValidator()
        {
            RuleFor(x => x.ManufacturerName)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
