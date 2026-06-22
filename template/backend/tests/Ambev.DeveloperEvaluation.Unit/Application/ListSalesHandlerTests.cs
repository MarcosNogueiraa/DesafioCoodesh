using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Unit tests for <see cref="ListSalesHandler"/>.
/// </summary>
public class ListSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly ListSalesHandler _handler;

    public ListSalesHandlerTests()
    {
        _handler = new ListSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given sales When listing Then returns page with metadata")]
    public async Task Handle_ReturnsPaginatedResult()
    {
        // Arrange
        var sales = new List<Sale> { new() { Id = Guid.NewGuid() }, new() { Id = Guid.NewGuid() } };
        _saleRepository.ListAsync(1, 10, null, Arg.Any<CancellationToken>())
            .Returns((sales, 42));
        _mapper.Map<List<SaleResult>>(Arg.Any<IReadOnlyList<Sale>>())
            .Returns(new List<SaleResult> { new(), new() });

        // Act
        var result = await _handler.Handle(new ListSalesCommand(1, 10), CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(42);
        result.CurrentPage.Should().Be(1);
        result.Sales.Should().HaveCount(2);
    }

    [Fact(DisplayName = "Given invalid page When listing Then throws ValidationException")]
    public async Task Handle_InvalidPage_Throws()
    {
        // Act
        var act = () => _handler.Handle(new ListSalesCommand(0, 10), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
