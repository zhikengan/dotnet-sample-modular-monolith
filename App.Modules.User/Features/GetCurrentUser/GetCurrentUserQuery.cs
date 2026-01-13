using App.Shared.Infrastructure;

namespace App.Modules.User.Features.GetCurrentUser;

// Query
public record GetCurrentUserQuery(Guid UserId) : IQuery<UserDto>;
