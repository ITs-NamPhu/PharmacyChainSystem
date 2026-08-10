using FluentValidation;
using PharmacyManagement.DTOs.Warehouse;

namespace PharmacyManagement.Validators.FluentValidation.Warehouse
{
    public class UpdateWarehouseValidator
        : AbstractValidator<UpdateWarehouseRequest>
    {
        public UpdateWarehouseValidator()
        {
            RuleFor(x => x.WarehouseName)
                .NotEmpty()
                .MaximumLength(255);
        }
    }
}
