using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a single product line within a <see cref="Sale"/>.
/// References the product through the External Identities pattern
/// (denormalized <see cref="ProductId"/> + <see cref="ProductName"/>) and
/// encapsulates the quantity-based discount business rules.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Maximum quantity of identical items allowed in a single line.
    /// </summary>
    public const int MaxQuantity = 20;

    /// <summary>
    /// Gets the identifier of the parent sale.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets the external identifier of the referenced product.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets the denormalized description (name) of the referenced product.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the quantity of identical products in this line.
    /// Must be between 1 and <see cref="MaxQuantity"/>.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets the unit price of the product at the time of the sale.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets the monetary discount applied to this line, derived from the
    /// quantity-based discount tiers. Calculated by <see cref="ApplyDiscountRules"/>.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets the total amount for this line: (Quantity * UnitPrice) - Discount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Indicates whether this item has been cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Calculates the discount rate that applies to a given quantity, according
    /// to the business rules:
    /// <list type="bullet">
    /// <item>Below 4 items: no discount.</item>
    /// <item>4 to 9 items: 10% discount.</item>
    /// <item>10 to 20 items: 20% discount.</item>
    /// </list>
    /// </summary>
    /// <param name="quantity">The quantity of identical items.</param>
    /// <returns>The discount rate as a fraction (0, 0.10 or 0.20).</returns>
    public static decimal GetDiscountRate(int quantity)
    {
        if (quantity >= 10)
            return 0.20m;
        if (quantity >= 4)
            return 0.10m;
        return 0m;
    }

    /// <summary>
    /// Applies the quantity-based discount business rules to this item and
    /// recalculates <see cref="Discount"/> and <see cref="TotalAmount"/>.
    /// </summary>
    /// <exception cref="DomainException">
    /// Thrown when <see cref="Quantity"/> exceeds <see cref="MaxQuantity"/>.
    /// </exception>
    public void ApplyDiscountRules()
    {
        if (Quantity > MaxQuantity)
            throw new DomainException($"It's not possible to sell above {MaxQuantity} identical items.");

        var grossAmount = Quantity * UnitPrice;
        Discount = grossAmount * GetDiscountRate(Quantity);
        TotalAmount = grossAmount - Discount;
    }

    /// <summary>
    /// Marks this item as cancelled.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
    }

    /// <summary>
    /// Performs validation of the sale item using the <see cref="SaleItemValidator"/> rules.
    /// </summary>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleItemValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}
