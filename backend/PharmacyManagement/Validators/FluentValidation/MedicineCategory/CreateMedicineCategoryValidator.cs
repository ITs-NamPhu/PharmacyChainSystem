using FluentValidation;
using PharmacyManagement.DTOs.MedicineCategory;

namespace PharmacyManagement.Validators.FluentValidation.MedicineCategory
{
    public class CreateMedicineCategoryValidator
        : AbstractValidator<CreateMedicineCategoryRequest>
    {
        public CreateMedicineCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
