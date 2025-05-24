
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P02_SalesDatabase.Data;
using P02_SalesDatabase.Models;
using System.ComponentModel.DataAnnotations;

namespace P02_SalesDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection().AddDbContext<SalesContext>(options => options.Use_InMemoryDatabase("SalesDatabase")).BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<SalesContext>())
            {
                DbSeeder.Seed(context);
                Console.WriteLine("Sales Database seeded successfully!");
            }
        }
    }

}


namespace P02_SalesDatabase.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public double Quantity { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; } = "No description";

        public ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();
    }

}


namespace P02_SalesDatabase.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(80)]
        public string Email { get; set; }

        public string CreaditCardNumber { get; set; }

        public ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();
    }

}


namespace P02_SalesDatabase.Models
{
    public class Store
    {
        public int StoreId { get; set; }

        [MaxLength(80)]
        public string Name { get; set; }

        public ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();
    }

}



namespace P02_SalesDatabase.Models
{
    public class Sale
    {
        public int SaleId { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; }
    }

}


namespace P02_SalesDatabase.Data
{
    public class SalesContext : DbContext
    {
        public SalesContext(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(p => p.Description).HasDefaultValue("No description");
            modelBuilder.Entity<Product>().Property(p => p.Name).IsUnicode(true);
            modelBuilder.Entity<Customer>().Property(c => c.Email).IsUnicode(false);
            modelBuilder.Entity<Sale>().Property(s => s.Date).HasDefaultValueSql("GETDATE()");
        }
    }

}



namespace P02_SalesDatabase.Data
{
    public static class DbSeeder
    {
        public static void Seed(SalesContext context)
        {
            var customer = new Customer { Name = "Omar", Email = "omar@email.com", CreaditCardNumber = "1234-5678-9012" }; var product = new Product { Name = "Laptop", Quantity = 10, Price = 15000 }; var store = new Store { Name = "Tech Store" }; var sale = new Sale { Customer = customer, Product = product, Store = store };

            context.Customers.Add(customer);
            context.Products.Add(product);
            context.Stores.Add(store);
            context.Sales.Add(sale);

            context.SaveChanges();
        }
    }

}