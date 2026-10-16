using Domain.Common.Entities;
using Domain.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Servico : SoftDeleteBaseEntity
    {
        public string Nome { get; private set; }

        public Servico(string nome)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nome);

            this.Nome = nome;
        }

        public Servico(Guid id, string nome)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nome);

            this.Nome = nome;
        }

        public void Update(string nome)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nome);

            Nome = nome.Trim();
        }
    }
}
