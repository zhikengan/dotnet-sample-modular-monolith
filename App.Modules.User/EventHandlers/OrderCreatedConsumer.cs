using App.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace App.Modules.User.EventHandlers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderCreated> context)
    {
        _logger.LogInformation("Event Received: Order {OrderId} created for User {UserId} with amount {Amount}", 
            context.Message.OrderId, context.Message.UserId, context.Message.TotalAmount);

        // Throw error for testing purposes
        throw new Exception("Simulated exception in OrderCreatedConsumer");

        // return Task.CompletedTask;
    }
}
