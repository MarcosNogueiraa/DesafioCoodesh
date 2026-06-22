using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Maps the UpdateSale API request to the application command.
/// </summary>
public class UpdateSaleRequestProfile : Profile
{
    public UpdateSaleRequestProfile()
    {
        CreateMap<UpdateSaleItemRequest, UpdateSaleItemCommand>();
        // Id is assigned in the controller from the route value.
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>()
            .ForMember(d => d.Id, opt => opt.Ignore());
    }
}
