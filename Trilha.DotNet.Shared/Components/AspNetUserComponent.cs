namespace Trilha.DotNet.Shared.Components;

public class AspNetUserComponent(IHttpContextAccessor accessor)
{
    public string? Name
        => accessor.HttpContext?.User.Identity?.Name;

    public string GetType(string type)
        => (IsAuthenticated() ? accessor.HttpContext?.User.FindFirst(type)!.Value : string.Empty) ?? string.Empty;

    public bool IsAuthenticated()
        => accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role)
        => accessor.HttpContext?.User.IsInRole(role) ?? false;

    public IEnumerable<Claim> GetClaimsIdentity()
        => accessor.HttpContext?.User.Claims ?? [];
}
