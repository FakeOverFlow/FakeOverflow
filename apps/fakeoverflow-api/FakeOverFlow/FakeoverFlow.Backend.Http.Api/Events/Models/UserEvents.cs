using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Events.Models;

public record UserSignupEvent(Guid UserId, string Email) : IEvent;