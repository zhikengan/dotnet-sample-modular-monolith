using FluentValidation;

namespace App.Modules.Ordering.Features.CreateOrder;

public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
