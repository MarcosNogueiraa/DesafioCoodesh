using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale record (aggregate root). Customers and branches are
/// referenced through the External Identities pattern (denormalized
/// identifier + description) and the sale owns its collection of
/// <see cref="SaleItem"/> lines.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets the human-readable sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets the date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets the external identifier of the customer.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets the denormalized description (name) of the customer.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the external identifier of the branch where the sale was made.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets the denormalized description (name) of the branch.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the total amount of the sale (sum of the totals of the
    /// non-cancelled items).
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Indicates whether the whole sale has been cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets the items that compose this sale.
    /// </summary>
    public List<SaleItem> Items { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Sale"/> class.
    /// </summary>
    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds an item to the sale, applies the discount business rules to it and
    /// recalculates the sale total.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void AddItem(SaleItem item)
    {
        item.ApplyDiscountRules();
        Items.Add(item);
        RecalculateTotal();
    }

    /// <summary>
    /// Recalculates the sale total as the sum of the totals of the
    /// non-cancelled items.
    /// </summary>
    public void RecalculateTotal()
    {
        TotalAmount = Items
            .Where(i => !i.IsCancelled)
            .Sum(i => i.TotalAmount);
    }

    /// <summary>
    /// Cancels the entire sale.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels a single item of the sale and recalculates the total.
    /// </summary>
    /// <param name="itemId">The identifier of the item to cancel.</param>
    /// <returns>The cancelled item, or <c>null</c> when the item was not found.</returns>
    public SaleItem? CancelItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
            return null;

        item.Cancel();
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
        return item;
    }

    /// <summary>
    /// Performs validation of the sale entity using the <see cref="SaleValidator"/> rules.
    /// </summary>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}
