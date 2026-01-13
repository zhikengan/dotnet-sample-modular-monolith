using App.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace App.Modules.Notification.Features.EventHandlers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderCreated> context)
    {
        _logger.LogInformation("Notification Module Received: Order {OrderId} created for User {UserId}. Sending Email...", 
            context.Message.OrderId, context.Message.UserId);

        return Task.CompletedTask;
    }
}
