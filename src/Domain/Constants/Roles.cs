namespace Domain.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Manager = "Manager";
    public const string Atendente = "Atendente";

    public static readonly string[] All = [Admin, User, Manager, Atendente];

    public static bool IsValid(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return false;

        return All.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
