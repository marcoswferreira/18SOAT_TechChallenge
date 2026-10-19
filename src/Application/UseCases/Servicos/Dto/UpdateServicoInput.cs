using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Servicos.Dto;

public record UpdateServicoInput(Guid Id,string Nome);
