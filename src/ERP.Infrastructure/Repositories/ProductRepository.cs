using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERP.Infrastructure.Data;
using ERP.Domain.Entities; // <-- Se o erro persistir, mude apenas esta linha para: using ERP.Domain;

namespace ERP.Infrastructure.Repositories
{
    public class ProductRepository
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public ProductRepository(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<Product>> GetProductsByTenantAsync(Guid tenantId)
        {
            string cacheKey = $"products_tenant_{tenantId}";

            // 1. Tenta buscar o JSON do Redis
            var cachedProductsJson = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedProductsJson))
            {
                // Se achou, converte o JSON de volta para a lista de produtos
                return JsonSerializer.Deserialize<List<Product>>(cachedProductsJson);
            }

            // 2. Se não achou no Redis, busca no PostgreSQL otimizado
            var products = await _context.Products
                                         .AsNoTracking()
                                         .Where(p => p.TenantId == tenantId)
                                         .ToListAsync();

            // 3. Converte os produtos do banco para JSON e salva no Redis por 2 minutos
            var cacheOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(2));

            string productsJson = JsonSerializer.Serialize(products);
            await _cache.SetStringAsync(cacheKey, productsJson, cacheOptions);

            return products;
        }
    }
}