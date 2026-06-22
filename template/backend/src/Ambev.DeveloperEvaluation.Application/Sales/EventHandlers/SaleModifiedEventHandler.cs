using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.EventHandlers;

/// <summary>
/// Logs the <see cref="SaleModifiedEvent"/>.
/// </summary>
public class SaleModifiedEventHandler : INotificationHandler<SaleModifiedEvent>
{
    private readonly ILogger<SaleModifiedEventHandler> _logger;

    public SaleModifiedEventHandler(ILogger<SaleModifiedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[SaleModified] SaleId={SaleId} SaleNumber={SaleNumber} TotalAmount={TotalAmount}",
            notification.Sale.Id, notification.Sale.SaleNumber, notification.Sale.TotalAmount);
        return Task.CompletedTask;
    }
}
