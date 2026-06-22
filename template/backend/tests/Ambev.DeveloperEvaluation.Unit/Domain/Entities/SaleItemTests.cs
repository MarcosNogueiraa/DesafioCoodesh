using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Unit tests for the <see cref="SaleItem"/> entity, focused on the
/// quantity-based discount business rules.
/// </summary>
public class SaleItemTests
{
    [Theory(DisplayName = "Discount rate should follow the quantity tiers")]
    [InlineData(1, 0.00)]
    [InlineData(3, 0.00)]
    [InlineData(4, 0.10)]
    [InlineData(9, 0.10)]
    [InlineData(10, 0.20)]
    [InlineData(20, 0.20)]
    public void Given_Quantity_When_GetDiscountRate_Then_ReturnsExpectedRate(int quantity, decimal expectedRate)
    {
        // Act
        var rate = SaleItem.GetDiscountRate(quantity);

        // Assert
        rate.Should().Be(expectedRate);
    }

    [Theory(DisplayName = "ApplyDiscountRules should compute discount and total per tier")]
    [InlineData(3, 100, 0, 300)]      // below 4: no discount
    [InlineData(4, 100, 40, 360)]     // 10% of 400
    [InlineData(10, 100, 200, 800)]   // 20% of 1000
    [InlineData(20, 50, 200, 800)]    // 20% of 1000
    public void Given_Item_When_ApplyDiscountRules_Then_SetsDiscountAndTotal(
        int quantity, decimal unitPrice, decimal expectedDiscount, decimal expectedTotal)
    {
        // Arrange
        var item = new SaleItem { Quantity = quantity, UnitPrice = unitPrice };

        // Act
        item.ApplyDiscountRules();

        // Assert
        item.Discount.Should().Be(expectedDiscount);
        item.TotalAmount.Should().Be(expectedTotal);
    }

    [Fact(DisplayName = "ApplyDiscountRules should throw when quantity exceeds the maximum")]
    public void Given_QuantityAboveMaximum_When_ApplyDiscountRules_Then_ThrowsDomainException()
    {
        // Arrange
        var item = new SaleItem { Quantity = 21, UnitPrice = 100 };

        // Act
        var act = () => item.ApplyDiscountRules();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*20 identical items*");
    }

    [Fact(DisplayName = "Cancel should mark the item as cancelled")]
    public void Given_Item_When_Cancelled_Then_IsCancelledIsTrue()
    {
        // Arrange
        var item = new SaleItem { Quantity = 5, UnitPrice = 10 };

        // Act
        item.Cancel();

        // Assert
        item.IsCancelled.Should().BeTrue();
    }
}
