using Microsoft.EntityFrameworkCore;
using ERP.Domain.Entities;

namespace ERP.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // GERAÇÃO DO ÍNDICE:
            // Isso cria uma "agenda telefônica" no PostgreSQL para o TenantId
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.TenantId)
                .HasDatabaseName("IX_Products_TenantId");
        }
    }
}