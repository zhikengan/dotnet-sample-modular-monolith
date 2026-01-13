using App.Shared.Infrastructure;
using App.Shared.Models;

namespace App.Modules.Ordering.Features.CreateOrder;

// UserId is set internally from JWT token, not from request body
public record CreateOrderCommand(decimal Amount) : ICommand<StdResponse<Guid>>
{
    public Guid UserId { get; init; }
}
