using System;

namespace ERP.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty; // Código da peça (Ex: CAM-PRE-M)
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        
        // A trava de segurança Multi-Tenant (Garante que uma empresa não veja o estoque da outra)
        public Guid TenantId { get; set; } 

        public Product() { }

        public Product(string name, string sku, int quantity, decimal price, Guid tenantId)
        {
            Name = name;
            Sku = sku;
            Quantity = quantity;
            Price = price;
            TenantId = tenantId;
        }
    }
}