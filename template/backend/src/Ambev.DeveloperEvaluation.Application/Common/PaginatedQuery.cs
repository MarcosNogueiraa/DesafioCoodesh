namespace Ambev.DeveloperEvaluation.Application.Common;

/// <summary>
/// Base type for queries that return paginated results. It centralizes the
/// common pagination and sorting attributes so future list queries can reuse
/// them simply by inheriting from this class.
/// </summary>
public abstract class PaginatedQuery
{
    /// <summary>
    /// The 1-based page number to retrieve. Defaults to 1.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// The number of items per page. Defaults to 10.
    /// </summary>
    public int Size { get; set; } = 10;

    /// <summary>
    /// Optional ordering expression (e.g. "saledate desc, salenumber asc").
    /// </summary>
    public string? Order { get; set; }
}
