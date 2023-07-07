namespace Trilha.DotNet.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AccessKeyAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _key;

    public AccessKeyAttribute(string key)
    {
        _key = key;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var headers = context.HttpContext.Request.Headers;

        var isValid = headers.ContainsKey("AccessKey") && headers["AccessKey"].ToString() == _key;

        if (!isValid)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                Message = "Invalid Access Key"
            });
        }
    }
}