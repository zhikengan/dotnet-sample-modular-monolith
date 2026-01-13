using System.ComponentModel.DataAnnotations;

namespace App.Modules.Catalog.Data;

public class Product
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public int Stock { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
