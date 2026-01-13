using App.Shared.Infrastructure;
using App.Shared.Models;

namespace App.Modules.Catalog.Features.GetProducts;

public record GetProductsQuery(int Page = 1, int PageSize = 10) : IQuery<PagedResult<ProductDto>>;
