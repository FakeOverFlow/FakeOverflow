namespace FakeoverFlow.Backend.Abstraction.Models;

public interface IPostAuditableEntity
{
    public Guid CreatedBy { get; protected set; }
    
    public DateTimeOffset CreatedOn { get; protected set; }

    void AuditCreation(Guid createdBy, DateTimeOffset? current)
    {
        CreatedOn = current ?? DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
    }
}