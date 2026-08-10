using FluentValidation;
using PharmacyManagement.DTOs.Unit;

namespace PharmacyManagement.Validators.FluentValidation.Unit
{
    public class CreateUnitValidator
        : AbstractValidator<CreateUnitRequest>
    {
        public CreateUnitValidator()
        {
            RuleFor(x => x.UnitName)
                .NotEmpty()
                .MaximumLength(255);
        }
    }
}
