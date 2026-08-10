using FluentValidation;
using PharmacyManagement.DTOs.Supplier;

namespace PharmacyManagement.Validators.FluentValidation.Supplier
{
    public class UpdateSupplierValidator
        : AbstractValidator<UpdateSupplierRequest>
    {
        public UpdateSupplierValidator()
        {
            RuleFor(x => x.SupplierName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .Matches(@"^[0-9]{10}$");

            RuleFor(x => x.Address)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .EmailAddress();
        }
    }
}
