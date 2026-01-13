using App.Modules.User.Data;
using App.Modules.User.Features;
using App.Modules.User.Features.LoginUser;
using App.Modules.User.Features.RegisterUser;
using App.Modules.User.Features.GetCurrentUser;
using App.Shared.Contacts;
using App.Shared.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace App.Modules.User;

public static class UserModule
{
    public static IServiceCollection AddUserModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserModuleFacade, UserModuleApi>();
        
        // Validators
        services.AddValidatorsFromAssembly(typeof(UserModule).Assembly);

        // MediatR Handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(UserModule).Assembly);
        });

        return services;
    }

    public static void MapUserModuleEndpoints(this IEndpointRouteBuilder app)
    {
        var userGroup = app.MapGroup("api/users")
            .WithTags("Users")
            .WithAppDefaults();

        userGroup.MapRegisterUserEndpoint();
        userGroup.MapLoginUserEndpoint();
        userGroup.MapGetCurrentUserEndpoint();
    }
}
