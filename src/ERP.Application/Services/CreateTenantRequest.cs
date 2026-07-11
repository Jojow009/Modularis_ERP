namespace ERP.Application.Services
{
    public class CreateTenantRequest
    {
        // Inicializar com string.Empty remove os avisos amarelos
        public string Name { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
    }
}