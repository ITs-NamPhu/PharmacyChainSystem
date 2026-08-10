using FluentValidation;
using PharmacyManagement.DTOs.CustomerType;

namespace PharmacyManagement.Validators.FluentValidation.CustomerType
{
    public class UpdateCustomerTypeValidator
        : AbstractValidator<UpdateCustomerTypeRequest>
    {
        public UpdateCustomerTypeValidator()
        {
            RuleFor(x => x.TypeName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.DiscountPercent)
                .InclusiveBetween(0, 100);
        }
    }
}
