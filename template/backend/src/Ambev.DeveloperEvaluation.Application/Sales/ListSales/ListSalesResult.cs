using Ambev.DeveloperEvaluation.Application.Sales.Common;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Result of the ListSales operation carrying the page of sales and the
/// pagination metadata.
/// </summary>
public class ListSalesResult
{
    public IReadOnlyList<SaleResult> Sales { get; set; } = new List<SaleResult>();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}
