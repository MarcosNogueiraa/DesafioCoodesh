using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Centralizes Bogus-based generation of valid <see cref="Sale"/> and
/// <see cref="SaleItem"/> instances for the domain entity tests.
/// </summary>
public static class SaleTestData
{
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 1000));

    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .RuleFor(s => s.SaleNumber, f => f.Random.Replace("SALE-#####"))
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Person.FullName)
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName());

    /// <summary>
    /// Generates a valid sale item with the given quantity (default randomized
    /// within the allowed range). Discount rules are not applied automatically.
    /// </summary>
    public static SaleItem GenerateValidItem(int? quantity = null)
    {
        var item = SaleItemFaker.Generate();
        if (quantity.HasValue)
            item.Quantity = quantity.Value;
        return item;
    }

    /// <summary>
    /// Generates a valid sale containing the requested number of items, each one
    /// already added through the aggregate (so discounts and totals are applied).
    /// </summary>
    public static Sale GenerateValidSale(int itemCount = 3)
    {
        var sale = SaleFaker.Generate();
        for (var i = 0; i < itemCount; i++)
            sale.AddItem(GenerateValidItem());
        return sale;
    }
}
