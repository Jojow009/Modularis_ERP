namespace ERP.API.DTOs // O endereço agora bate com a pasta
{
    public class LoginRequest 
    {
        public string Email { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
    }
}