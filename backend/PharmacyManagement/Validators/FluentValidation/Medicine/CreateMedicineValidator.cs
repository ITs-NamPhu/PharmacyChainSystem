using FluentValidation;
using PharmacyManagement.DTOs.Medicine;

namespace PharmacyManagement.Validators.FluentValidation.Medicine
{
    public class CreateMedicineValidator
        : AbstractValidator<CreateMedicineRequest>
    {
        public CreateMedicineValidator()
        {
            RuleFor(x => x.MedicineName)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.DefaultRetailPrice)
                .GreaterThan(0);

            RuleFor(x => x.DefaultWholesalePrice)
                .GreaterThan(0);

            RuleFor(x => x.VATPercent)
                .InclusiveBetween(0, 100);

            RuleFor(x => x.CategoryID)
                .GreaterThan(0);

            RuleFor(x => x.ManufacturerID)
                .GreaterThan(0);

            RuleFor(x => x.BaseUnitID)
                .GreaterThan(0);
        }
    }
}
