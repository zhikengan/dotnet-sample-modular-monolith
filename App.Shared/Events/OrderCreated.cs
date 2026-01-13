namespace App.Shared.Events;

public record OrderCreated(Guid OrderId, Guid UserId, decimal TotalAmount);
