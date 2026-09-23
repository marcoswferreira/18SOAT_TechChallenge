namespace Domain.Common.Entities;

/// <summary>
/// Herda de BaseEntity e adiciona rastreabilidade de criação e modificação. 
/// Ideal para tabelas transacionais (como User, Order, Payment).
/// </summary>
public abstract class AuditableBaseEntity : BaseEntity
{
    public DateTime CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string? LastModifiedBy { get; private set; }

    protected AuditableBaseEntity(Guid id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    protected AuditableBaseEntity() : base()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public void SetCreatedInfo(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void SetUpdatedInfo(string modifiedBy)
    {
        LastModifiedAt = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}