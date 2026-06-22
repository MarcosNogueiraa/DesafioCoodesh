using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
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
/// Unit tests for <see cref="CreateSaleHandler"/>.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _mediator);

        // Map the command to a Sale carrying its scalar fields (without items).
        _mapper.Map<Sale>(Arg.Any<CreateSaleCommand>()).Returns(ci =>
        {
            var c = (CreateSaleCommand)ci[0];
            return new Sale
            {
                SaleNumber = c.SaleNumber,
                SaleDate = c.SaleDate,
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName,
                BranchId = c.BranchId,
                BranchName = c.BranchName
            };
        });

        _mapper.Map<SaleItem>(Arg.Any<CreateSaleItemCommand>()).Returns(ci =>
        {
            var c = (CreateSaleItemCommand)ci[0];
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

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(ci => (Sale)ci[0]);
    }

    [Fact(DisplayName = "Given valid command When creating sale Then persists, applies discount and publishes event")]
    public async Task Handle_ValidCommand_CreatesSale()
    {
        // Arrange
        var command = SaleHandlerTestData.GenerateValidCreateCommand();
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(800); // qty 10 * 100, 20% discount
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _mediator.Received(1).Publish(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given duplicate sale number When creating sale Then throws InvalidOperationException")]
    public async Task Handle_DuplicateSaleNumber_Throws()
    {
        // Arrange
        var command = SaleHandlerTestData.GenerateValidCreateCommand();
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(new Sale());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact(DisplayName = "Given invalid command When creating sale Then throws ValidationException")]
    public async Task Handle_InvalidCommand_Throws()
    {
        // Arrange
        var command = new CreateSaleCommand(); // no items, missing required fields

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
