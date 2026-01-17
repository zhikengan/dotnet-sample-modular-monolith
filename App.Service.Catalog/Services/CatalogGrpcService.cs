using Grpc.Core;
using MediatR;
using App.Shared.Protos;
using App.Modules.Catalog.Features.CreateProduct;
using App.Modules.Catalog.Features.GetProducts;

namespace App.Service.Catalog.Services;

public class CatalogGrpcService : CatalogService.CatalogServiceBase
{
    private readonly ISender _sender;
    private readonly ILogger<CatalogGrpcService> _logger;

    public CatalogGrpcService(ISender sender, ILogger<CatalogGrpcService> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public override async Task<GetProductsResponse> GetProducts(GetProductsRequest request, ServerCallContext context)
    {
        var query = new GetProductsQuery(request.Page, request.PageSize);
        var result = await _sender.Send(query);

        var response = new GetProductsResponse
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };

        response.Items.AddRange(result.Items.Select(p => new Product
        {
            Id = p.Id.ToString(),
            Name = p.Name,
            Description = p.Description,
            Price = p.Price.ToString("F2"), // String for precision
            Stock = p.Stock
        }));

        return response;
    }

    public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        if (!decimal.TryParse(request.Price, out var price))
        {
             throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid price format"));
        }

        var command = new CreateProductCommand(request.Name, request.Description, price, request.Stock);
        var productId = await _sender.Send(command);

        return new CreateProductResponse
        {
            Id = productId.ToString()
        };
    }
}
