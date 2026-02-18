using System;

namespace ERP.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public Guid TenantId { get; private set; }
        
        public bool IsEmailVerified { get; private set; } = false;
        public string? VerificationToken { get; private set; }

        // --- CAMPOS DE LGPD E AUDITORIA ---
        public bool IsDeleted { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        // Construtor vazio exigido pelo Entity Framework Core
        private User() { } 

        // Construtor Rico (DDD)
        public User(string email, string password, Guid tenantId)
        {
            Id = Guid.NewGuid();
            Email = email;
            TenantId = tenantId;
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            VerificationToken = Guid.NewGuid().ToString();
        }

        // Comportamentos (Métodos)
        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }

        public void ConfirmEmail()
        {
            IsEmailVerified = true;
            VerificationToken = null; 
        }

        // Método para LGPD (Soft Delete)
        public void InactivateAccount()
        {
            IsDeleted = true;
        }
    }
}