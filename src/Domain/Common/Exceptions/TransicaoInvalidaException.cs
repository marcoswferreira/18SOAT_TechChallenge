using Domain.Enums;

namespace Domain.Common;

public sealed class TransicaoInvalidaException : DomainException
{
    public TransicaoInvalidaException(StatusOrdemServico de, StatusOrdemServico para)
        : base($"Transição inválida: de '{de}' para '{para}'.")
    { }
}