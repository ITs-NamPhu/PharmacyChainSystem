using FluentValidation;
using PharmacyManagement.DTOs.Manufacturer;

namespace PharmacyManagement.Validators.FluentValidation.Manufacturer
{
    public class CreateManufacturerValidator
        : AbstractValidator<CreateManufacturerRequest>
    {
        public CreateManufacturerValidator()
        {
            RuleFor(x => x.ManufacturerName)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
