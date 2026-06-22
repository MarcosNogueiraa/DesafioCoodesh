using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Unit tests for <see cref="UpdateSaleHandler"/>.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _mediator);

        // In-place mapping of scalar fields from command to the existing sale.
        _mapper.When(m => m.Map(Arg.Any<UpdateSaleCommand>(), Arg.Any<Sale>()))
            .Do(ci =>
            {
                var c = (UpdateSaleCommand)ci[0];
                var s = (Sale)ci[1];
                s.SaleNumber = c.SaleNumber;
                s.CustomerName = c.CustomerName;
            });

        _mapper.Map<SaleItem>(Arg.Any<UpdateSaleItemCommand>()).Returns(ci =>
        {
            var c = (UpdateSaleItemCommand)ci[0];
            return new SaleItem
            {
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                Quantity = c.Quantity,
                UnitPrice = c.UnitPrice
            };
        });

        _mapper.Map<SaleResult>(Arg.Any<Sale>()).Returns(ci =>
        {
            var s = (Sale)ci[0];
            return new SaleResult { Id = s.Id, SaleNumber = s.SaleNumber, TotalAmount = s.TotalAmount };
        });

        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(ci => (Sale)ci[0]);
    }

    [Fact(DisplayName = "Given existing sale When updating Then replaces items, recalculates and publishes event")]
    public async Task Handle_ExistingSale_Updates()
    {
        // Arrange
        var existing = new Sale { Id = Guid.NewGuid(), SaleNumber = "OLD" };
        existing.AddItem(new SaleItem { ProductId = Guid.NewGuid(), ProductName = "old", Quantity = 1, UnitPrice = 10 });
        _saleRepository.GetByIdAsync(existing.Id, Arg.Any<CancellationToken>()).Returns(existing);

        var command = SaleHandlerTestData.GenerateValidUpdateCommand(existing.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.SaleNumber.Should().Be("SALE-UPDATED");
        existing.Items.Should().HaveCount(1);
        result.TotalAmount.Should().Be(450); // qty 5 * 100, 10% discount
        await _mediator.Received(1).Publish(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given missing sale When updating Then throws KeyNotFoundException")]
    public async Task Handle_MissingSale_Throws()
    {
        // Arrange
        _saleRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Sale?)null);
        var command = SaleHandlerTestData.GenerateValidUpdateCommand(Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
