using FluentValidation;
using PharmacyManagement.DTOs.GoodsReceipt;

namespace PharmacyManagement.Validators.FluentValidation.GoodsReceipt
{
    public class CreateGoodsReceiptValidator
        : AbstractValidator<CreateGoodsReceiptRequest>
    {
        public CreateGoodsReceiptValidator()
        {
            RuleFor(x => x.SupplierID)
                .GreaterThan(0);

            RuleFor(x => x.PaidAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.ReceiptDate)
                .NotEmpty();

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("At least one item is required.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.MedicineID).GreaterThan(0);
                item.RuleFor(i => i.UnitID).GreaterThan(0);
                item.RuleFor(i => i.Quantity).GreaterThan(0);
                item.RuleFor(i => i.UnitCost).GreaterThan(0);
            });
        }
    }
}
