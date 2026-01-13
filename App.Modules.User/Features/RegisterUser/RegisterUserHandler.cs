using App.Modules.User.Data;
using App.Shared.Exceptions;
using App.Shared.Infrastructure;
using App.Shared.Models;
using App.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace App.Modules.User.Features.RegisterUser;

public class RegisterUserHandler : ICommandHandler<RegisterUserCommand, StdResponse<Guid>>
{
    private readonly UserDbContext _dbContext;

    public RegisterUserHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StdResponse<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _dbContext.Users.AnyAsync(u => u.Username == request.Username, cancellationToken);
        Assert.False(userExists, ErrorCodes.USER_EXISTS);

        var user = new Data.User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            PasswordHash = request.Password // Hash in real app
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return StdResponse<Guid>.Ok(user.Id);
    }
}
