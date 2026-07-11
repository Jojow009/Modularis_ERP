using System;

namespace ERP.Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Cnpj { get; private set; }

        private Tenant() { } // Construtor para o Entity Framework

        public Tenant(string name, string cnpj)
        {
            Id = Guid.NewGuid();
            Name = name;
            Cnpj = cnpj;
        }
    }
}