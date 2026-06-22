using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Query to retrieve a paginated, optionally ordered list of sales.
/// Pagination attributes (Page, Size, Order) are inherited from
/// <see cref="PaginatedQuery"/> and shared with any future list query.
/// </summary>
public class ListSalesCommand : PaginatedQuery, IRequest<ListSalesResult>
{
    public ListSalesCommand() { }

    public ListSalesCommand(int page, int size, string? order = null)
    {
        Page = page;
        Size = size;
        Order = order;
    }
}
