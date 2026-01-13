using App.Shared.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace App.Shared.Extensions;

public static class EndpointExtensions
{
    public static RouteGroupBuilder WithAppDefaults(this RouteGroupBuilder group)
    {
        return group
            .RequireAuthorization()
            .AddEndpointFilter<ResponseWrapperFilter>();
    }
}
