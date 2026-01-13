namespace App.Modules.Ordering.Data;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}
