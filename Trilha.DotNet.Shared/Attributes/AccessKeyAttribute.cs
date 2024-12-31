namespace Trilha.DotNet.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AccessKeyAttribute(string key, string value) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var headers = context.HttpContext.Request.Headers;

        if (!headers.TryGetValue(key, out var accessKey) || accessKey != value)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                Message = "Invalid Access Key"
            });
        }
    }
}