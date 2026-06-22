using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Centralizes Bogus-based generation of valid sale commands for the
/// application handler tests.
/// </summary>
public static class SaleHandlerTestData
{
    /// <summary>
    /// Generates a valid CreateSale command. By default it contains a single
    /// item with quantity 10 and unit price 100 (a deterministic 20% discount
    /// tier, total 800) so totals can be asserted.
    /// </summary>
    public static CreateSaleCommand GenerateValidCreateCommand()
    {
        return new Faker<CreateSaleCommand>()
            .RuleFor(c => c.SaleNumber, f => f.Random.Replace("SALE-#####"))
            .RuleFor(c => c.SaleDate, f => f.Date.Recent())
            .RuleFor(c => c.CustomerId, f => f.Random.Guid())
            .RuleFor(c => c.CustomerName, f => f.Person.FullName)
            .RuleFor(c => c.BranchId, f => f.Random.Guid())
            .RuleFor(c => c.BranchName, f => f.Company.CompanyName())
            .RuleFor(c => c.Items, _ => new List<CreateSaleItemCommand>
            {
                new() { ProductId = Guid.NewGuid(), ProductName = "Product", Quantity = 10, UnitPrice = 100 }
            })
            .Generate();
    }

    /// <summary>
    /// Generates a valid UpdateSale command for the given sale id.
    /// </summary>
    public static UpdateSaleCommand GenerateValidUpdateCommand(Guid saleId)
    {
        return new UpdateSaleCommand
        {
            Id = saleId,
            SaleNumber = "SALE-UPDATED",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch",
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = Guid.NewGuid(), ProductName = "Product", Quantity = 5, UnitPrice = 100 }
            }
        };
    }
}
