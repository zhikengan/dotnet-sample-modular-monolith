using App.Modules.Catalog.Data;
using App.Shared.Infrastructure;
using App.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.Catalog.Features.GetProducts;

public class GetProductsHandler : IQueryHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly CatalogDbContext _dbContext;

    public GetProductsHandler(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        Console.WriteLine("GetProductsHandler");

        var query = _dbContext.Products.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductDto>(items, request.Page, request.PageSize, totalCount);
    }
}
