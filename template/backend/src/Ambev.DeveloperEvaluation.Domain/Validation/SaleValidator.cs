using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validates a <see cref="Sale"/> ensuring it carries the required external
/// identities, at least one item, and that every item is valid.
/// </summary>
public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(sale => sale.SaleNumber)
            .NotEmpty().WithMessage("Sale number is required.")
            .MaximumLength(50);

        RuleFor(sale => sale.SaleDate)
            .NotEmpty().WithMessage("Sale date is required.");

        RuleFor(sale => sale.CustomerId)
            .NotEmpty().WithMessage("Customer identifier is required.");

        RuleFor(sale => sale.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(100);

        RuleFor(sale => sale.BranchId)
            .NotEmpty().WithMessage("Branch identifier is required.");

        RuleFor(sale => sale.BranchName)
            .NotEmpty().WithMessage("Branch name is required.")
            .MaximumLength(100);

        RuleFor(sale => sale.Items)
            .NotEmpty().WithMessage("A sale must contain at least one item.");

        RuleForEach(sale => sale.Items).SetValidator(new SaleItemValidator());
    }
}
