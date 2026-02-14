using Microsoft.AspNetCore.Mvc;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using System.Threading.Tasks;

namespace ERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TenantsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TenantRequest request)
        {
            // Pega os dados que vieram do Next.js e cria a Entidade
            var tenant = new Tenant(request.Name, request.Cnpj);
            
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            // Retorna a empresa criada (com o ID gerado) para o Frontend
            return Ok(tenant);
        }
    }

    // A MÁGICA ESTÁ AQUI: O "molde" dos dados que vêm do Frontend (Next.js)
    public class TenantRequest 
    {
        public string Name { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
    }
}