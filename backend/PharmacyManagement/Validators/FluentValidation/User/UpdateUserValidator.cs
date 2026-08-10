using FluentValidation;
using PharmacyManagement.DTOs.User;

namespace PharmacyManagement.Validators.FluentValidation.User
{
    public class UpdateUserValidator
        : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.Phone)
                .Matches(@"^[0-9]{10}$")
                .MaximumLength(10);

            RuleFor(x => x.Email)
                .EmailAddress();
        }
    }
}
