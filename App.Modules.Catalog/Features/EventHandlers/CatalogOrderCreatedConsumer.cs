using App.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace App.Modules.Catalog.Features.EventHandlers;

public class CatalogOrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly ILogger<CatalogOrderCreatedConsumer> _logger;

    public CatalogOrderCreatedConsumer(ILogger<CatalogOrderCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<OrderCreated> context)
    {
        _logger.LogInformation("Catalog Service received OrderCreated event: OrderId={OrderId}, UserId={UserId}", 
            context.Message.OrderId, context.Message.UserId);

        // TODO: Implement business logic (e.g., reserve stock, update analytics)
        
        return Task.CompletedTask;
    }
}
