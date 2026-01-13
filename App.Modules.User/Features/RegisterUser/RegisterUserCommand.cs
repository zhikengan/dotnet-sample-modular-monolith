using App.Shared.Infrastructure;
using App.Shared.Models;

namespace App.Modules.User.Features.RegisterUser;

public record RegisterUserCommand(string Username, string Password) : ICommand<StdResponse<Guid>>;
