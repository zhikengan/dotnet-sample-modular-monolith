using MassTransit;

namespace App.Modules.User.EventHandlers;

public class OrderCreatedConsumerDefinition : ConsumerDefinition<OrderCreatedConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<OrderCreatedConsumer> consumerConfigurator, IRegistrationContext context)
    {
        // Specific Retry Policy: Retry 3 times with a 2-second interval
        endpointConfigurator.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));
    }
}
