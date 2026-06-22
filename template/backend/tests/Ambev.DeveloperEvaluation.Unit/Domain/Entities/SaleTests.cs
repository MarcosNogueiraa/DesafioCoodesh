using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Unit tests for the <see cref="Sale"/> aggregate root.
/// </summary>
public class SaleTests
{
    [Fact(DisplayName = "AddItem should apply discount rules and update the total")]
    public void Given_Sale_When_AddItem_Then_TotalReflectsItem()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale(0);
        var item = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "X", Quantity = 10, UnitPrice = 100 };

        // Act
        sale.AddItem(item);

        // Assert
        item.Discount.Should().Be(200);     // 20% of 1000
        sale.TotalAmount.Should().Be(800);
    }

    [Fact(DisplayName = "RecalculateTotal should ignore cancelled items")]
    public void Given_SaleWithCancelledItem_When_Recalculated_Then_IgnoresCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale(0);
        sale.AddItem(new SaleItem { ProductId = Guid.NewGuid(), ProductName = "A", Quantity = 5, UnitPrice = 100 });
        sale.AddItem(new SaleItem { ProductId = Guid.NewGuid(), ProductName = "B", Quantity = 5, UnitPrice = 100 });
        var expectedTotalOfFirst = sale.Items[0].TotalAmount;

        // Act
        sale.Items[1].Cancel();
        sale.RecalculateTotal();

        // Assert
        sale.TotalAmount.Should().Be(expectedTotalOfFirst);
    }

    [Fact(DisplayName = "Cancel should mark the whole sale as cancelled")]
    public void Given_Sale_When_Cancelled_Then_IsCancelledIsTrue()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.Cancel();

        // Assert
        sale.IsCancelled.Should().BeTrue();
        sale.UpdatedAt.Should().NotBeNull();
    }

    [Fact(DisplayName = "CancelItem should cancel the item and recalculate the total")]
    public void Given_Sale_When_CancelItem_Then_ItemCancelledAndTotalUpdated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale(0);
        sale.AddItem(new SaleItem { ProductId = Guid.NewGuid(), ProductName = "A", Quantity = 5, UnitPrice = 100 });
        sale.AddItem(new SaleItem { ProductId = Guid.NewGuid(), ProductName = "B", Quantity = 5, UnitPrice = 100 });
        var target = sale.Items[0];
        var remainingTotal = sale.Items[1].TotalAmount;

        // Act
        var cancelled = sale.CancelItem(target.Id);

        // Assert
        cancelled.Should().NotBeNull();
        target.IsCancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(remainingTotal);
    }

    [Fact(DisplayName = "CancelItem should return null for an unknown item")]
    public void Given_Sale_When_CancelUnknownItem_Then_ReturnsNull()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var cancelled = sale.CancelItem(Guid.NewGuid());

        // Assert
        cancelled.Should().BeNull();
    }
}
