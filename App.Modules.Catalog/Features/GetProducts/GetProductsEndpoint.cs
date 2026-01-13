using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;

namespace App.Modules.Catalog.Features.GetProducts;

public static class GetProductsEndpoint
{
    public static void MapGetProductsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async ([AsParameters] GetProductsQuery query, ISender sender) =>
        {
            return await sender.Send(query);
            // ResponseWrapper will detect PagedResult<T> and convert to StdPagedResponse<T>
        });
    }
}
