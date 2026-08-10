using FluentValidation;
using PharmacyManagement.DTOs.UnitConversion;

namespace PharmacyManagement.Validators.FluentValidation.UnitConversion
{
    public class UpdateUnitConversionValidator
        : AbstractValidator<UpdateUnitConversionRequest>
    {
        public UpdateUnitConversionValidator()
        {
            RuleFor(x => x.Factor)
                .GreaterThan(0);
        }
    }
}
