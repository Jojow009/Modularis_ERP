using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Necessário para o [Authorize]
using Microsoft.Extensions.Caching.Distributed;
using ERP.Infrastructure.Data;
using ERP.Infrastructure.Repositories;
using ERP.Domain.Entities; 
using System.Security.Claims;

namespace ERP.API.Controllers
{
    // DTO colocado aqui dentro para evitar o erro CS0246
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    [Authorize] // Bloqueia acesso sem Token
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductRepository _repository;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public ProductsController(ProductRepository repository, AppDbContext context, IDistributedCache cache)
        {
            _repository = repository;
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // Extrai o TenantId do Token assinado digitalmente
            var tenantClaim = User.FindFirst("tenantId")?.Value;
            if (string.IsNullOrEmpty(tenantClaim)) return Unauthorized();

            var tenantId = Guid.Parse(tenantClaim);
            var products = await _repository.GetProductsByTenantAsync(tenantId);
            
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProductRequest request)
        {
            var tenantClaim = User.FindFirst("tenantId")?.Value;
            var tenantId = Guid.Parse(tenantClaim!);

            // Usando o construtor rico do DDD (Domain-Driven Design)
            var product = new Product(
                request.Name, 
                request.Sku, 
                request.Quantity, 
                request.Price, 
                tenantId
            );

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            await _cache.RemoveAsync($"products_tenant_{tenantId}");

            return Ok(new { message = "Produto criado com sucesso!", product });
        }
    }
}