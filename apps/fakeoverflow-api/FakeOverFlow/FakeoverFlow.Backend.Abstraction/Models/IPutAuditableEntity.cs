namespace FakeoverFlow.Backend.Abstraction.Models;

public interface IPutAuditableEntity
{
    public Guid UpdatedBy { get; protected set; }
    
    public DateTimeOffset UpdatedOn { get; protected set; }
    
    void AuditModification(Guid updatedBy, DateTimeOffset? current)
    {
        UpdatedOn = current ?? DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy;
    }
}