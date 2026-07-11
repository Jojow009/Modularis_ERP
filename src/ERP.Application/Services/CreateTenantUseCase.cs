using ERP.Application.Interfaces;
using ERP.Domain.Entities;

namespace ERP.Application.Services
{
    public class CreateTenantUseCase
    {
        private readonly ITenantRepository _repository;

        public CreateTenantUseCase(ITenantRepository repository) => _repository = repository;

        public async Task ExecuteAsync(CreateTenantRequest request)
        {
            var tenant = new Tenant(request.Name, request.Cnpj);
            await _repository.AddAsync(tenant);
        }
    }
}