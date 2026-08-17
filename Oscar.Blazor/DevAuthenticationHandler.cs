using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Oscar.Blazor
{
    /// <summary>
    /// Development-only authentication handler that automatically signs in a fake local user,
    /// bypassing Azure AD. Only registered when ASPNETCORE_ENVIRONMENT is Development.
    /// </summary>
    public class DevAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "DevAuth";

        public DevAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
#pragma warning disable CS0618 // Type or member is obsolete
            : base(options, logger, encoder, clock)
#pragma warning restore CS0618
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "dev.user@localhost"),
                new Claim(ClaimTypes.Email, "dev.user@localhost"),
                new Claim("name", "Dev User"),
                new Claim("preferred_username", "dev.user@localhost")
            };

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
