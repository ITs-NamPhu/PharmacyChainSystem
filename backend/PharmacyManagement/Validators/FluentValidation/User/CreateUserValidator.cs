using FluentValidation;
using PharmacyManagement.DTOs.User;

namespace PharmacyManagement.Validators.FluentValidation.User
{
    public class CreateUserValidator
        : AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8);

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
