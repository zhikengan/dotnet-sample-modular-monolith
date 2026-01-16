using App.Modules.User.Data;
using App.Shared.Exceptions;
using App.Shared.Infrastructure;
using App.Shared.Utils;

namespace App.Modules.User.Features.GetCurrentUser;

// Handler
public class GetCurrentUserHandler : IQueryHandler<GetCurrentUserQuery, UserDto>
{
    private readonly UserDbContext _dbContext;

    public GetCurrentUserHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FindAsync(request.UserId);
        Assert.NotNull(user, errorCode: ErrorCodes.USER_NOT_FOUND);
        return new UserDto(user.Id, user.Username);
    }
}
