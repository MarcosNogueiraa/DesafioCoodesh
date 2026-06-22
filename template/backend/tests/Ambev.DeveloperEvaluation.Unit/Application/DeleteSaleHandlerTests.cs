using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Unit tests for <see cref="DeleteSaleHandler"/>.
/// </summary>
public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository = Substitute.For<ISaleRepository>();
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _handler = new DeleteSaleHandler(_saleRepository);
    }

    [Fact(DisplayName = "Given existing sale When deleting Then returns success")]
    public async Task Handle_ExistingSale_ReturnsSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        _saleRepository.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _handler.Handle(new DeleteSaleCommand(id), CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "Given missing sale When deleting Then throws KeyNotFoundException")]
    public async Task Handle_MissingSale_Throws()
    {
        // Arrange
        _saleRepository.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = () => _handler.Handle(new DeleteSaleCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
