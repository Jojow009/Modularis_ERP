using Microsoft.AspNetCore.Mvc;
using ERP.Domain.Entities;
using ERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context) => _context = context;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { message = "Este e-mail já está em uso." });

            var user = new User(request.Email, request.Password, request.TenantId);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // SIMULAÇÃO DE ENVIO DE E-MAIL
            // Em produção, você usaria MailKit, SendGrid ou AWS SES aqui.
            var linkVerificacao = $"http://localhost:3000/verify-email?token={user.VerificationToken}";
            Console.WriteLine($"\n[EMAIL MOCK] Para: {user.Email} -> Clique para verificar: {linkVerificacao}\n");

            return Ok(new { message = "Usuário criado! Verifique seu e-mail para ativar a conta." });
        }

        // NOVO ENDPOINT: Valida o token
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == request.Token);

            if (user == null)
                return BadRequest(new { message = "Token inválido ou já utilizado." });

            user.ConfirmEmail(); // Altera o status e limpa o token
            await _context.SaveChangesAsync();

            return Ok(new { message = "E-mail verificado com sucesso! Você já pode fazer login." });
        }
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
    }

    public class VerifyRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}