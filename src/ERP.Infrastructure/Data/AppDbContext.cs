using Microsoft.EntityFrameworkCore;
using ERP.Domain.Entities;

namespace ERP.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
         public DbSet<ERP.Domain.Entities.Product> Products { get; set; }

        public DbSet<Tenant> Tenants { get; set; }
        
        // A sintaxe correta e simplificada é esta:
        public DbSet<User> Users { get; set; }
    }
}

