using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Unit tests for <see cref="CancelSaleItemHandler"/>.
/// </summary>
public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _handler = new CancelSaleItemHandler(_saleRepository, _mediator);
    }

    private static Sale SaleWithItem(out Guid itemId)
    {
        var sale = new Sale { Id = Guid.NewGuid() };
        sale.AddItem(new SaleItem { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), ProductName = "P", Quantity = 5, UnitPrice = 100 });
        itemId = sale.Items[0].Id;
        return sale;
    }

    [Fact(DisplayName = "Given existing item When cancelling Then cancels item and publishes event")]
    public async Task Handle_ExistingItem_CancelsAndPublishes()
    {
        // Arrange
        var sale = SaleWithItem(out var itemId);
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var result = await _handler.Handle(new CancelSaleItemCommand(sale.Id, itemId), CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        sale.Items[0].IsCancelled.Should().BeTrue();
        await _mediator.Received(1).Publish(Arg.Any<ItemCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given missing item When cancelling Then throws KeyNotFoundException")]
    public async Task Handle_MissingItem_Throws()
    {
        // Arrange
        var sale = SaleWithItem(out _);
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var act = () => _handler.Handle(new CancelSaleItemCommand(sale.Id, Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
