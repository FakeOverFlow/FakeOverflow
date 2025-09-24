using FakeoverFlow.Backend.Http.Api.Events.Models;
using FastEndpoints;

namespace FakeoverFlow.Backend.Http.Api.Events.Handlers;

public class UserSignupEventHandler : IEventHandler<UserSignupEvent>
{
    
    private readonly ILogger<UserSignupEventHandler> _logger;

    public UserSignupEventHandler(ILogger<UserSignupEventHandler> logger)
    {
        _logger = logger;
    }

    public  async Task HandleAsync(UserSignupEvent eventModel, CancellationToken ct)
    {
        
    }
}