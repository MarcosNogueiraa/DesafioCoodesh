using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Raised when a new sale is created.
/// </summary>
public class SaleCreatedEvent : INotification
{
    public Sale Sale { get; }

    public SaleCreatedEvent(Sale sale)
    {
        Sale = sale;
    }
}
