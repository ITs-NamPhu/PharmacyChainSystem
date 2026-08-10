using FluentValidation;
using PharmacyManagement.DTOs.Customer;

namespace PharmacyManagement.Validators.FluentValidation.Customer
{
    public class UpdateCustomerValidator
        : AbstractValidator<UpdateCustomerRequest>
    {
        public UpdateCustomerValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .Matches(@"^[0-9]{10}$");

            RuleFor(x => x.Address)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.CustomerTypeID)
                .GreaterThan(0);
        }
    }
}
