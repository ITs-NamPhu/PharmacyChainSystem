using FluentValidation;
using PharmacyManagement.DTOs.Unit;

namespace PharmacyManagement.Validators.FluentValidation.Unit
{
    public class UpdateUnitValidator
        : AbstractValidator<UpdateUnitRequest>
    {
        public UpdateUnitValidator()
        {
            RuleFor(x => x.UnitName)
                .NotEmpty()
                .MaximumLength(255);
        }
    }
}
