using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace App.Modules.User.Features.LoginUser;

public static class LoginUserEndpoint
{
    public static void MapLoginUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async ([FromBody] LoginUserCommand command, ISender sender) =>
        {
            var response = await sender.Send(command);
            return response.Success ? Results.Ok(response) : Results.Unauthorized();
        })
        .AllowAnonymous();
    }
}
