using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when a single item of a sale is cancelled.
/// </summary>
public class ItemCancelledEvent : INotification
{
    public Sale Sale { get; }
    public SaleItem Item { get; }

    public ItemCancelledEvent(Sale sale, SaleItem item)
    {
        Sale = sale;
        Item = item;
    }
}
