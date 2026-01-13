using App.Shared.Infrastructure;

namespace App.Modules.Catalog.Features.CreateProduct;

public record CreateProductCommand(string Name, string Description, decimal Price, int Stock) : ICommand<Guid>;
