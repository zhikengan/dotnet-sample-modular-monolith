using Microsoft.EntityFrameworkCore;

namespace App.Modules.Catalog.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configuration
        modelBuilder.Entity<Product>().HasKey(p => p.Id);
        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
    }
}
