using FluentValidation;
using PharmacyManagement.DTOs.Role;

namespace PharmacyManagement.Validators.FluentValidation.Role
{
    public class UpdateRoleValidator
        : AbstractValidator<UpdateRoleRequest>
    {
        public UpdateRoleValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty()
                .MaximumLength(255);
        }
    }
}
