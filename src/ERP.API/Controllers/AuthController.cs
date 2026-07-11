using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERP.Infrastructure.Data;
using ERP.API.DTOs;
using BCrypt.Net; // Essencial para validar a senha criptografada do usuário!

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
            // 1. Busca o usuário pelo e-mail
            // Usamos o IgnoreQueryFilters() porque ainda não sabemos a qual empresa ele pertence
            var user = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                // Devolvemos uma mensagem genérica por segurança contra ataques de força bruta
                return Unauthorized(new { message = "E-mail ou senha incorretos." });
            }

            // 2. Compara a senha digitada com o Hash que está salvo no banco do PostgreSQL
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "E-mail ou senha incorretos." });
            }

            // 3. Agora que confirmamos a identidade, buscamos a empresa dele para pegar o Nome Fantasia
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == user.TenantId);

            // 4. Devolvemos tudo o que a nossa página de login no Next.js espera salvar no localStorage
            return Ok(new 
            { 
                message = "Acesso concedido", 
                token = "jwt_token_temporario", // Aqui você conectará a sua geração real de token JWT
                tenantId = user.TenantId,
                tenantName = tenant?.Name ?? "Empresa",
                email = user.Email 
            });
        }
    }
}