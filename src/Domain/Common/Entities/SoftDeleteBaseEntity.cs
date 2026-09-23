namespace Domain.Common.Entities;

/// <summary>
/// Para entidades que não podem ter os dados fisicamente apagados do banco de dados por 
/// compliance, histórico ou integridade referencial.
/// </summary>
public abstract class SoftDeleteBaseEntity : AuditableBaseEntity
{
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    public void Delete(string deletedBy)
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}
