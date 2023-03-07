namespace Trilha.DotNet.Shared.Components;

public class AspNetUserComponent
{
    private readonly IHttpContextAccessor _accessor;

    public AspNetUserComponent(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string? Name
        => _accessor.HttpContext.User.Identity?.Name;

    public string GetType(string type)
        => (IsAuthenticated() ? _accessor.HttpContext?.User.FindFirst(type)!.Value : string.Empty) ?? string.Empty;

    public bool IsAuthenticated()
        => _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role)
        => _accessor.HttpContext.User.IsInRole(role);

    public IEnumerable<Claim> GetClaimsIdentity()
        => _accessor.HttpContext.User.Claims;
}
