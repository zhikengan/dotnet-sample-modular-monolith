using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;

namespace App.Modules.Catalog.Features.CreateProduct;

public static class CreateProductEndpoint
{
    public static void MapCreateProductEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", async ([FromBody] CreateProductCommand command, ISender sender) =>
        {
            var productId = await sender.Send(command);
            // Returns 201 Created with the ID. 
            // The ResponseWrapper will wrap this in StdResponse<Guid>.Ok(productId)
            return Results.Created($"/api/products/{productId}", productId);
        });
    }
}
