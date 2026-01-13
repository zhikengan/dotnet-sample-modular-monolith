namespace App.Modules.Catalog.Features.GetProducts;

public record ProductDto(Guid Id, string Name, string Description, decimal Price, int Stock);
