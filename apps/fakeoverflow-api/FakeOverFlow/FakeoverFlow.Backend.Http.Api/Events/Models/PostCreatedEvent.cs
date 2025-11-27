using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Events.Models;

public record PostCreatedEvent(string PostTitle, string PostContent, string PostId) : IEvent;