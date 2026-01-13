using App.Modules.Catalog.Data;
using App.Shared.Infrastructure;

namespace App.Modules.Catalog.Features.CreateProduct;

public class CreateProductHandler : ICommandHandler<CreateProductCommand, Guid>
{
    private readonly CatalogDbContext _dbContext;

    public CreateProductHandler(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
