using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Unit tests for <see cref="SaleItemValidator"/>.
/// </summary>
public class SaleItemValidatorTests
{
    private readonly SaleItemValidator _validator = new();

    private static SaleItem ValidItem() => new()
    {
        ProductId = Guid.NewGuid(),
        ProductName = "Product",
        Quantity = 5,
        UnitPrice = 10
    };

    [Fact(DisplayName = "Valid item should pass validation")]
    public void Given_ValidItem_When_Validated_Then_NoErrors()
    {
        var result = _validator.TestValidate(ValidItem());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Quantity above 20 should fail validation")]
    public void Given_QuantityAboveMaximum_When_Validated_Then_HasError()
    {
        var item = ValidItem();
        item.Quantity = 21;
        var result = _validator.TestValidate(item);
        result.ShouldHaveValidationErrorFor(i => i.Quantity);
    }

    [Fact(DisplayName = "Zero quantity should fail validation")]
    public void Given_ZeroQuantity_When_Validated_Then_HasError()
    {
        var item = ValidItem();
        item.Quantity = 0;
        var result = _validator.TestValidate(item);
        result.ShouldHaveValidationErrorFor(i => i.Quantity);
    }

    [Fact(DisplayName = "Non-positive unit price should fail validation")]
    public void Given_NonPositiveUnitPrice_When_Validated_Then_HasError()
    {
        var item = ValidItem();
        item.UnitPrice = 0;
        var result = _validator.TestValidate(item);
        result.ShouldHaveValidationErrorFor(i => i.UnitPrice);
    }
}
