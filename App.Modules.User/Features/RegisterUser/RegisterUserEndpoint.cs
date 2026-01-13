using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace App.Modules.User.Features.RegisterUser;

public static class RegisterUserEndpoint
{
    public static void MapRegisterUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/register", async ([FromBody] RegisterUserCommand command, ISender sender) =>
        {
            var response = await sender.Send(command);
            return response.Success ? Results.Ok(response) : Results.BadRequest(response);
        })
        .AllowAnonymous();
    }
}
