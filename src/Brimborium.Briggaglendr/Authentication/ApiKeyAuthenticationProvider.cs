using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;

namespace Brimborium.Briggaglendr.Authentication;

/// <summary>
/// checks if the http-header called "x-apikey" is set to a configured value.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        throw new NotImplementedException();
    }
}

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// Valid ApiKey Value
    /// </summary>
    public string ApiKeyValue { get; set; } = string.Empty;

    /// <summary>
    /// Validate the ApiKey Value
    /// </summary>
    /// <value></value>
    public Func<string, bool>? ValidateApiKey { get; set; }
}