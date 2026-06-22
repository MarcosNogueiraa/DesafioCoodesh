using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// AutoMapper profile for the CreateSale operation.
/// </summary>
public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        // Items are added through the Sale aggregate so the discount rules run.
        CreateMap<CreateSaleCommand, Sale>()
            .ForMember(d => d.Items, opt => opt.Ignore());

        CreateMap<CreateSaleItemCommand, SaleItem>();
    }
}
