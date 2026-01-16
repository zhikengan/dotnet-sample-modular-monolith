using App.Modules.Ordering.Data;
using App.Modules.Ordering.Features.CreateOrder;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using App.Shared.Extensions;
using Microsoft.AspNetCore.Http;

namespace App.Modules.Ordering;

public static class OrderingModule
{
    public static IServiceCollection AddOrderingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderingDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Validators
        services.AddValidatorsFromAssembly(typeof(OrderingModule).Assembly);

        // MediatR Handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(OrderingModule).Assembly);
        });

        return services;
    }

    public static void MapOrderingModuleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/orders")
            .WithTags("Orders")
            .WithAppDefaults();
        
        group.MapCreateOrderEndpoint();
    }
}
