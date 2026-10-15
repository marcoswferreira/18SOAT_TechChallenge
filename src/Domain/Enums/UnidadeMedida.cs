using System.Text.Json.Serialization;

namespace Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UnidadeMedida
{
    Unidade = 1,
    Litro = 2,
    Quilo = 3
}
