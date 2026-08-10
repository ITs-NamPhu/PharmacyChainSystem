using FluentValidation;
using PharmacyManagement.DTOs.User;

namespace PharmacyManagement.Validators.FluentValidation.User
{
    public class UserAssignmentValidator
        : AbstractValidator<UserAssignmentRequest>
    {
        public UserAssignmentValidator()
        {
            RuleFor(x => x.RoleId)
                .GreaterThan(0);

        }
    }
}
