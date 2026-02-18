using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERP.Infrastructure.Data;
using ERP.API.DTOs; // A ponte agora está funcionando

namespace ERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // CONSULTA REAL: Verifica se o ID existe no PostgreSQL
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == request.TenantId);

            if (tenant == null)
            {
                // Se o código for inventado, o C# barra aqui!
                return Unauthorized(new { message = "Código de empresa inválido!" });
            }

            // Se a empresa existe (como a Griffo do CNPJ 57.789.329/0001-09), liberamos
            return Ok(new { message = "Acesso concedido", tenantName = tenant.Name });
        }
    }
}