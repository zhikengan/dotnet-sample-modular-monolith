using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using App.Shared.Extensions;
using App.Shared.Utils;

namespace App.Modules.User.Features.GetCurrentUser;

public static class GetCurrentUserEndpoint
{
    public static void MapGetCurrentUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/me", async (ClaimsPrincipal user, ISender sender) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isParsed = Guid.TryParse(userIdClaim, out var userId);
            
            Assert.Authentication(!string.IsNullOrEmpty(userIdClaim) && isParsed, "User ID not found in token");

            var query = new GetCurrentUserQuery(userId);
            return await sender.Send(query); 
        });
    }
}
