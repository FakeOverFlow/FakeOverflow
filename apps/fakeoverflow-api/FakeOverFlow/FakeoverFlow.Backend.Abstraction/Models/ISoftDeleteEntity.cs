namespace FakeoverFlow.Backend.Abstraction.Models;

public interface ISoftDeleteEntity
{
    public bool IsDeleted { get; set; }
}