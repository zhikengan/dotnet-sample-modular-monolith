using App.Shared.Protos;
using Microsoft.AspNetCore.Mvc;
using Grpc.Net.Client;

namespace App.Web.Endpoints;

public static class CatalogEndpoints
{
    public record CreateProductRequestDto(string Name, string Description, decimal Price, int Stock);
    public record GetProductsQueryDto(int Page = 1, int PageSize = 10);

    public static void MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/products")
            .WithTags("Products");
            // .WithAppDefaults(); // Assuming available extension

        group.MapGet("/", async ([AsParameters] GetProductsQueryDto query, CatalogService.CatalogServiceClient client) =>
        {
            var request = new GetProductsRequest
            {
                Page = query.Page,
                PageSize = query.PageSize
            };
            var response = await client.GetProductsAsync(request);
            
            // Map Back to DTO/Result to avoid exposing Proto types directly
            var mappedItems = response.Items.Select(p => new 
            {
                Id = Guid.Parse(p.Id),
                Name = p.Name,
                Description = p.Description,
                Price = decimal.TryParse(p.Price, out var d) ? d : 0m,
                Stock = p.Stock
            });

            return new 
            {
                Items = mappedItems,
                Page = response.Page,
                PageSize = response.PageSize,
                TotalCount = response.TotalCount
            };
        });

        group.MapPost("/", async ([FromBody] CreateProductRequestDto request, CatalogService.CatalogServiceClient client) =>
        {
            var protoRequest = new CreateProductRequest
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price.ToString("F2"),
                Stock = request.Stock
            };
            var response = await client.CreateProductAsync(protoRequest);
            return Results.Created($"/api/products/{response.Id}", Guid.Parse(response.Id));
        });
    }
}
