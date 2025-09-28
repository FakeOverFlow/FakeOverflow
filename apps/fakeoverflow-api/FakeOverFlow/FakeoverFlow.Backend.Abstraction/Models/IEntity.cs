namespace FakeoverFlow.Backend.Abstraction.Models;

public interface IEntity<TId>
{
    public TId Id { get; set; }
}