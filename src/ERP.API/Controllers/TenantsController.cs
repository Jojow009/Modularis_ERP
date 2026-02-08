using Microsoft.AspNetCore.Mvc;
using ERP.Application.Services;
using ERP.Domain.Entities;

namespace ERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantsController : ControllerBase
    {
        private readonly CreateTenantUseCase _createTenantUseCase;

        public TenantsController(CreateTenantUseCase createTenantUseCase)
        {
            _createTenantUseCase = createTenantUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Tenant tenant)
        {
            if (tenant == null) return BadRequest();
            
            // Este mapeamento resolve o erro de conversão CS1503
            var request = new CreateTenantRequest 
            { 
                Name = tenant.Name, 
                Cnpj = tenant.Cnpj 
            };
            
            // Verifique se o seu UseCase possui o método ExecuteAsync
            await _createTenantUseCase.ExecuteAsync(request);
            
            return Ok(new { message = "Empresa cadastrada com sucesso!" });
        }
    }
}