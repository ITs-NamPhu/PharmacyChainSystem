using FluentValidation;
using PharmacyManagement.DTOs.Branch;

namespace PharmacyManagement.Validators.FluentValidation.Branch
{
    public class CreateBranchValidator
        : AbstractValidator<CreateBranchRequest>
    {
        public CreateBranchValidator()
        {
            RuleFor(x => x.BranchName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .Matches(@"^[0-9]{10}$");

            RuleFor(x => x.Address)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.PriceListID)
                .GreaterThan(0);
        }
    }
}
