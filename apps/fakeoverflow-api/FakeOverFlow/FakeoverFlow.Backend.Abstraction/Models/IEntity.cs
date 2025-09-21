namespace FakeoverFlow.Backend.Abstraction.Models;

public interface IEntity
{
    public Guid Id { get; protected set; }
    
}