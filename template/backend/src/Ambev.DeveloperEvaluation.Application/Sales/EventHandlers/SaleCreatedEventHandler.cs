using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.EventHandlers;

/// <summary>
/// Logs the <see cref="SaleCreatedEvent"/>. In a real system this is where the
/// message would be published to a message broker.
/// </summary>
public class SaleCreatedEventHandler : INotificationHandler<SaleCreatedEvent>
{
    private readonly ILogger<SaleCreatedEventHandler> _logger;

    public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[SaleCreated] SaleId={SaleId} SaleNumber={SaleNumber} TotalAmount={TotalAmount}",
            notification.Sale.Id, notification.Sale.SaleNumber, notification.Sale.TotalAmount);
        return Task.CompletedTask;
    }
}
