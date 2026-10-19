using Application.UseCases.Clientes.Dto;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Servicos.Dto;

public record ServicoOutput(
    Guid Id,
    string Nome,
    DateTime CreatedAt
)

{
    public static ServicoOutput FromEntity(Servico servico) => new(
        servico.Id,
        servico.Nome,
        servico.CreatedAt
    );
}
