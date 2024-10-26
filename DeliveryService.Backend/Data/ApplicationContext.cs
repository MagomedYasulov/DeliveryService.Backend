using DeliveryService.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace DeliveryService.Backend.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Order> Orders => Set<Order>();

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
