using Microsoft.EntityFrameworkCore;
using Vilau_Paula_Proiect.Models;


namespace Vilau_Paula_Proiect.Data
{
    public class ToyStoreContext : DbContext
    {
        public ToyStoreContext(DbContextOptions<ToyStoreContext> options) : base(options) { 
        }

        public DbSet<Toy> Toys { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderedToy> OrderedToys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Toy>().ToTable("Toy");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Supplier>().ToTable("Supplier");
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Review>().ToTable("Review");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<OrderedToy>().HasKey(c => new { c.ToyID, c.OrderId });//configureaza cheia primara compusa
        }
        public DbSet<Vilau_Paula_Proiect.Models.Category> Category { get; set; } = default!;
        public DbSet<Vilau_Paula_Proiect.Models.Review> Review { get; set; } = default!;
        public DbSet<Vilau_Paula_Proiect.Models.Client> Client { get; set; } = default!;
        public DbSet<Vilau_Paula_Proiect.Models.Supplier> Supplier { get; set; } = default!;
    }
}
