using App.Modules.Ordering.Data;
using App.Shared.Contacts;
using App.Shared.Events;
using App.Shared.Exceptions;
using App.Shared.Infrastructure;
using App.Shared.Models;
using App.Shared.Utils;
using MassTransit;

namespace App.Modules.Ordering.Features.CreateOrder;

public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, StdResponse<Guid>>
{
    private readonly OrderingDbContext _dbContext;
    private readonly IUserModuleFacade _userModuleApi;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateOrderHandler(OrderingDbContext dbContext, IUserModuleFacade userModuleApi, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        _userModuleApi = userModuleApi;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<StdResponse<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Sync Invocation
        var userExists = await _userModuleApi.UserExistsAsync(request.UserId);
        Assert.True(userExists, ErrorCodes.USER_NOT_FOUND);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            TotalAmount = request.Amount,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Async Invocation
        await _publishEndpoint.Publish(new OrderCreated(order.Id, order.UserId, order.TotalAmount), cancellationToken);

        return StdResponse<Guid>.Ok(order.Id);
    }
}
