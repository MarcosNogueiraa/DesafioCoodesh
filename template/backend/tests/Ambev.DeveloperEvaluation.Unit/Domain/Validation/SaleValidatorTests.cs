using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Unit tests for <see cref="SaleValidator"/>.
/// </summary>
public class SaleValidatorTests
{
    private readonly SaleValidator _validator = new();

    [Fact(DisplayName = "Valid sale should pass validation")]
    public void Given_ValidSale_When_Validated_Then_NoErrors()
    {
        var sale = SaleTestData.GenerateValidSale();
        var result = _validator.TestValidate(sale);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Sale without items should fail validation")]
    public void Given_SaleWithoutItems_When_Validated_Then_HasError()
    {
        var sale = SaleTestData.GenerateValidSale(0);
        var result = _validator.TestValidate(sale);
        result.ShouldHaveValidationErrorFor(s => s.Items);
    }

    [Fact(DisplayName = "Sale without sale number should fail validation")]
    public void Given_SaleWithoutNumber_When_Validated_Then_HasError()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.SaleNumber = string.Empty;
        var result = _validator.TestValidate(sale);
        result.ShouldHaveValidationErrorFor(s => s.SaleNumber);
    }

    [Fact(DisplayName = "Sale without customer should fail validation")]
    public void Given_SaleWithoutCustomer_When_Validated_Then_HasError()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.CustomerId = Guid.Empty;
        var result = _validator.TestValidate(sale);
        result.ShouldHaveValidationErrorFor(s => s.CustomerId);
    }
}
