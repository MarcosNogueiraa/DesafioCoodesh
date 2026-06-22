using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for <see cref="UpdateSaleCommand"/>.
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<SaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale is null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        // Update scalar fields in place, then rebuild the items so the discount
        // rules are reapplied and the total recalculated.
        _mapper.Map(command, sale);

        sale.Items.Clear();
        foreach (var itemCommand in command.Items)
            sale.AddItem(_mapper.Map<SaleItem>(itemCommand));

        sale.UpdatedAt = DateTime.UtcNow;

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

        await _mediator.Publish(new SaleModifiedEvent(updatedSale), cancellationToken);

        return _mapper.Map<SaleResult>(updatedSale);
    }
}
