using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// AutoMapper profile for the UpdateSale operation.
/// </summary>
public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        // Update scalar fields only; the aggregate identity, audit fields and
        // items are handled explicitly in the handler.
        CreateMap<UpdateSaleCommand, Sale>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Items, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
            .ForMember(d => d.TotalAmount, opt => opt.Ignore())
            .ForMember(d => d.IsCancelled, opt => opt.Ignore());

        CreateMap<UpdateSaleItemCommand, SaleItem>();
    }
}
