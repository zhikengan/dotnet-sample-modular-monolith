using App.Shared.Infrastructure;
using App.Shared.Models;

namespace App.Modules.User.Features.LoginUser;

public record LoginUserCommand(string Username, string Password) : ICommand<StdResponse<string>>;
