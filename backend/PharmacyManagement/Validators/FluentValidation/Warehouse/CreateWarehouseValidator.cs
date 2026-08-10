using FluentValidation;
using PharmacyManagement.DTOs.Warehouse;

namespace PharmacyManagement.Validators.FluentValidation.Warehouse
{
    public class CreateWarehouseValidator
        : AbstractValidator<CreateWarehouseRequest>
    {
        public CreateWarehouseValidator()
        {
            RuleFor(x => x.WarehouseName)
                .NotEmpty()
                .MaximumLength(255);

        }
    }
}
