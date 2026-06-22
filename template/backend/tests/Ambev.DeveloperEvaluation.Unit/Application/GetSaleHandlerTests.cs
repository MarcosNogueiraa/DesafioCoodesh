using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Unit tests for <see cref="GetSaleHandler"/>.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _handler = new GetSaleHandler(_saleRepository, _mapper);
        _mapper.Map<SaleResult>(Arg.Any<Sale>())
            .Returns(ci => new SaleResult { Id = ((Sale)ci[0]).Id });
    }

    [Fact(DisplayName = "Given existing sale When getting Then returns it")]
    public async Task Handle_ExistingSale_ReturnsResult()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid() };
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var result = await _handler.Handle(new GetSaleCommand(sale.Id), CancellationToken.None);

        // Assert
        result.Id.Should().Be(sale.Id);
    }

    [Fact(DisplayName = "Given missing sale When getting Then throws KeyNotFoundException")]
    public async Task Handle_MissingSale_Throws()
    {
        // Arrange
        _saleRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var act = () => _handler.Handle(new GetSaleCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
