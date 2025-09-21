using NpgsqlTypes;

namespace FakeoverFlow.Backend.Abstraction.Models;

public interface IVectorSearchableEntity
{
    public NpgsqlTsVector VectorText { get; protected set; }
}