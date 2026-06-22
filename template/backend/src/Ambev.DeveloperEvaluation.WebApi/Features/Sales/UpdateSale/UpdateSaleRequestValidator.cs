using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Validator for <see cref="UpdateSaleRequest"/>.
/// </summary>
public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(r => r.SaleNumber).NotEmpty().MaximumLength(50);
        RuleFor(r => r.SaleDate).NotEmpty();
        RuleFor(r => r.CustomerId).NotEmpty();
        RuleFor(r => r.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(r => r.BranchId).NotEmpty();
        RuleFor(r => r.BranchName).NotEmpty().MaximumLength(100);
        RuleFor(r => r.Items).NotEmpty().WithMessage("A sale must contain at least one item.");

        RuleForEach(r => r.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.ProductName).NotEmpty().MaximumLength(100);
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
                .LessThanOrEqualTo(20).WithMessage("It's not possible to sell above 20 identical items.");
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}
