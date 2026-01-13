using App.Modules.User.Data;
using App.Shared.Infrastructure;

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
        
        if (user == null)
            throw new Exception("User not found"); // Should use BusinessException/NotFoundException

        return new UserDto(user.Id, user.Username);
    }
}
