using FakeoverFlow.Backend.Http.Api.Abstracts.DTO;

namespace FakeoverFlow.Backend.Http.Api.Abstracts.Services;

public interface IFactCheckerService
{
    Task<FactCheckResult> CheckFactAsync(string title, string content, string tags = "", CancellationToken cancellationToken = default);
}