using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERP.Infrastructure.Data;
using ERP.Application.DTOs;

namespace ERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context) => _context = context;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !user.VerifyPassword(request.Password))
                return Unauthorized(new { message = "E-mail ou senha inválidos." });

            return Ok(new { 
                token = "fake-jwt-token", 
                tenantId = user.TenantId,
                email = user.Email 
            });
        }
    }
}