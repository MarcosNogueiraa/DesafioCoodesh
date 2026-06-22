using Ambev.DeveloperEvaluation.Application.Sales.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

/// <summary>
/// Maps the application result DTOs to the API response DTOs.
/// </summary>
public class SaleResponseProfile : Profile
{
    public SaleResponseProfile()
    {
        CreateMap<SaleItemResult, SaleItemResponse>();
        CreateMap<SaleResult, SaleResponse>();
    }
}
