using FluentValidation;

namespace App.Modules.User.Features.RegisterUser;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
