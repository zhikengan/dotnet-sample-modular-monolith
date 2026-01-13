using App.Modules.Catalog.Data;
using App.Modules.Catalog.Features.CreateProduct;
using App.Modules.Catalog.Features.GetProducts;
using App.Shared.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Modules.Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddValidatorsFromAssembly(typeof(CatalogModule).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CatalogModule).Assembly);
        });

        return services;
    }

    public static void MapCatalogModuleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/products")
            .WithTags("Products")
            .WithAppDefaults(); // Auth + ResponseWrapper

        group.MapCreateProductEndpoint();
        group.MapGetProductsEndpoint();
    }
}
