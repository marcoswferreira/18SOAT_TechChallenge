using System;
using System.Collections.Generic;
using System.Text;

namespace Application.UseCases.Services.Dto;

public record UpdateServiceInput(Guid Id,string ServiceName);
