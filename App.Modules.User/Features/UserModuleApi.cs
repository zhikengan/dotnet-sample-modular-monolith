using App.Modules.User.Data;
using App.Shared.Contacts;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.User.Features;

public class UserModuleApi : IUserModuleFacade
{
    private readonly UserDbContext _dbContext;

    public UserModuleApi(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await _dbContext.Users.AnyAsync(x => x.Id == userId);
    }

    public async Task<string?> GetUserNameAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        return user?.Username;
    }
}
