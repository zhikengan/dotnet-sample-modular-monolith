using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace App.Modules.Ordering.Features.CreateOrder;

public static class CreateOrderEndpoint
{
    public static void MapCreateOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", async (
            [FromBody] CreateOrderCommand command,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            // Extract UserId from JWT token claims
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            // Set UserId from token
            var commandWithUser = command with { UserId = userId };
            
            var response = await sender.Send(commandWithUser);
            return Results.Ok(response);
        });
    }
}
