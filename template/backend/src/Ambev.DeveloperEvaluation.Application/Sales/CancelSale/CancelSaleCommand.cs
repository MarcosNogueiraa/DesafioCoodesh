using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Command to cancel an entire sale.
/// </summary>
public record CancelSaleCommand(Guid Id) : IRequest<CancelSaleResponse>;
