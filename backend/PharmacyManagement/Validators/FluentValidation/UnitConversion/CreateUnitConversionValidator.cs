using FluentValidation;
using PharmacyManagement.DTOs.UnitConversion;

namespace PharmacyManagement.Validators.FluentValidation.UnitConversion
{
    public class CreateUnitConversionValidator
        : AbstractValidator<CreateUnitConversionRequest>
    {
        public CreateUnitConversionValidator()
        {
            RuleFor(x => x.UnitID)
                .GreaterThan(0);

            RuleFor(x => x.MedicineID)
                .GreaterThan(0);

            RuleFor(x => x.Factor)
                .GreaterThan(0);
        }
    }
}
