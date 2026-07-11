using System;

namespace ERP.Domain.Entities
{
    public class Product
    {
        // Usando 'private set' para blindar a alteração de fora da classe
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Sku { get; private set; } 
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }
        public Guid TenantId { get; private set; } 

        // --- CAMPOS DE LGPD E AUDITORIA ---
        public bool IsDeleted { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Construtor vazio exigido pelo Entity Framework Core
        private Product() { }

        // Construtor Rico (DDD)
        public Product(string name, string sku, int quantity, decimal price, Guid tenantId)
        {
            Id = Guid.NewGuid();
            Name = name;
            Sku = sku;
            Quantity = quantity;
            Price = price;
            TenantId = tenantId;
        }

        // --- COMPORTAMENTOS (REGRAS DE NEGÓCIO E KARDEX) ---

        // Método para dar entrada no estoque
        public void AddStock(int amount)
        {
            if (amount <= 0) 
                throw new ArgumentException("A quantidade de entrada deve ser maior que zero.");
            
            Quantity += amount;
        }

        // Método para dar saída no estoque (Impede estoque negativo!)
        public void RemoveStock(int amount)
        {
            if (amount <= 0) 
                throw new ArgumentException("A quantidade de saída deve ser maior que zero.");
            
            if (Quantity - amount < 0) 
                throw new InvalidOperationException($"Estoque insuficiente. Você tentou remover {amount}, mas só há {Quantity} no estoque.");
            
            Quantity -= amount;
        }

        // Método para LGPD (Soft Delete)
        public void InactivateProduct()
        {
            IsDeleted = true;
        }
    }
}