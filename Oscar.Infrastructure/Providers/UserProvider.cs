using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Oscar.Core.Providers;

namespace Oscar.Infrastructure.Providers;

public class UserProvider : IUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetUserName() => _httpContextAccessor?.HttpContext?.User?.Identity?.Name;

    public string? GetName() =>  _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == "name")?.Value;

}