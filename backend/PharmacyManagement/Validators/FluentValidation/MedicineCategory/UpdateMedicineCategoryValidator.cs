using FluentValidation;
using PharmacyManagement.DTOs.MedicineCategory;

namespace PharmacyManagement.Validators.FluentValidation.MedicineCategory
{
    public class UpdateMedicineCategoryValidator
        : AbstractValidator<UpdateMedicineCategoryRequest>
    {
        public UpdateMedicineCategoryValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}
