using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Common;

public sealed class TransicaoInvalidaException : DomainException
{
    public TransicaoInvalidaException(StatusOrdemServico de, StatusOrdemServico para)
        : base($"Transição inválida: {de.Descricao()} → {para.Descricao()}. " +
               $"Próximos permitidos: {string.Join(", ", de.Proximos().Select(s => s.Descricao()))}")
    { }
}