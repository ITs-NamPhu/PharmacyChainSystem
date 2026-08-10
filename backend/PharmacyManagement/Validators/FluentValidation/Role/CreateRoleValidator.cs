using FluentValidation;
using PharmacyManagement.DTOs.Role;

namespace PharmacyManagement.Validators.FluentValidation.Role
{
    public class CreateRoleValidator
        : AbstractValidator<CreateRoleRequest>
    {
        public CreateRoleValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty()
                .MaximumLength(255);
        }
    }
}
