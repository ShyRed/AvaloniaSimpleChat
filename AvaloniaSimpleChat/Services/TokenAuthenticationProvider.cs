using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace AvaloniaSimpleChat.Services;

public sealed class TokenAuthenticationProvider : IAuthenticationProvider
{
    public string Token { get; set; } = string.Empty;
    
    public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (string.IsNullOrEmpty(Token))
            return Task.CompletedTask;
        
        request.Headers.TryAdd("Authorization", $"Bearer {Token}");
        return Task.CompletedTask;
    }
}