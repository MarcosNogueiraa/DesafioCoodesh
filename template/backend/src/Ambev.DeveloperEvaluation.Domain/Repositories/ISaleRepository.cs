using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository abstraction for persisting and querying <see cref="Sale"/> aggregates.
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Persists a new sale (together with its items).
    /// </summary>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its unique identifier, including its items.
    /// </summary>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its sale number, including its items.
    /// </summary>
    Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated, optionally ordered list of sales and the total count.
    /// </summary>
    /// <param name="page">The 1-based page number.</param>
    /// <param name="size">The page size.</param>
    /// <param name="order">An optional ordering expression (e.g. "saledate desc, salenumber asc").</param>
    Task<(IReadOnlyList<Sale> Sales, int TotalCount)> ListAsync(
        int page,
        int size,
        string? order = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sale (and its items).
    /// </summary>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale by its unique identifier.
    /// </summary>
    /// <returns><c>true</c> when the sale existed and was deleted; otherwise <c>false</c>.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
