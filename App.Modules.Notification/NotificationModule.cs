using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace App.Modules.Notification;

public static class NotificationModule
{
    public static Assembly Assembly => typeof(NotificationModule).Assembly;

    public static IServiceCollection AddNotificationModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register MediatR Handlers if any (currently none, but good practice to keep pattern)
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(NotificationModule).Assembly);
        });

        return services;
    }
}
