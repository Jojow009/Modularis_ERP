namespace ERP.Domain.Entities;

public class Tenant
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Cnpj { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Construtor para garantir que a entidade nasça válida
    public Tenant(string name, string cnpj)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome é obrigatório");
        if (cnpj?.Length != 14) throw new ArgumentException("CNPJ deve ter 14 dígitos");

        Id = Guid.NewGuid();
        Name = name;
        Cnpj = cnpj;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    // Método para desativar uma empresa (ex: falta de pagamento)
    public void Deactivate() => IsActive = false;
}