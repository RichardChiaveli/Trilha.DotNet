namespace Trilha.DotNet.Shared.Extensions;

public static class TokenExtensions
{
    private static SigningCredentials CreateTokenSignature(this string tokenSigning)
    {
        var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSigning));
        return new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature);
    }

    private static string FindFirst(this IEnumerable<Claim> claims, string key) =>
        claims.FirstOrDefault(i => i.Type == key)?.Value ?? string.Empty;

    public static string CreateJwt(
        this Guid userId
        , string userName
        , string userMail
        , string userPhone
        , string tokenSigning
        , int expirationMilliseconds
        , IHostingEnvironment environment
        , string audience = "All"
        , string role = "Default"
        , params KeyValuePair<string, string>[] claims)
    {
        var currentDate = DateTime.UtcNow;
        var expirationDate = currentDate.AddMilliseconds(expirationMilliseconds);

        var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Sid, userId.ToString()),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, userMail),
            new Claim(ClaimTypes.MobilePhone, userPhone),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Authentication, environment.EnvironmentName),
            new Claim(ClaimTypes.Expiration, expirationDate.Ticks.ToString())
        });

        foreach (var (key, value) in claims)
            identity.AddClaim(new Claim(key, value));

        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = environment.ApplicationName,
            Audience = audience,
            SigningCredentials = tokenSigning.CreateTokenSignature(),
            Subject = identity,
            NotBefore = currentDate,
            Expires = expirationDate
        });

        return handler.WriteToken(securityToken);
    }

    public static bool ValidateJwt(
        this string token
        , string tokenSigning
        , IHostingEnvironment environment
        , out IEnumerable<Claim> claims)
    {
        claims = new List<Claim>();

        if (string.IsNullOrWhiteSpace(token))
            return false;

        var jwtInput = token.Replace("Bearer", string.Empty).Trim();
        var jwtHandler = new JwtSecurityTokenHandler();

        if (!jwtHandler.CanReadToken(jwtInput))
            return false;

        var jwtToken = jwtHandler.ReadJwtToken(jwtInput);

        if (jwtToken.Claims.FindFirst(ClaimTypes.Authentication) != environment.EnvironmentName)
            return false;

        claims = jwtToken.Claims;

        var issuer = jwtToken.Claims.FindFirst(JwtRegisteredClaimNames.Iss);
        var audience = jwtToken.Claims.FindFirst(JwtRegisteredClaimNames.Aud);

        var paramsValidation = new TokenValidationParameters
        {
            IssuerSigningKey = tokenSigning.CreateTokenSignature().Key,
            ValidateIssuerSigningKey = true,
            ValidAudience = audience,
            ValidateAudience = true,
            ValidIssuer = issuer,
            ValidateIssuer = true,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true
        };

        static bool ValidateTokenSecurity(string token,
            ISecurityTokenValidator jwtHandler, TokenValidationParameters paramsValidation)
        {
            try
            {
                jwtHandler.ValidateToken(token, paramsValidation, out var validToken);

                if (validToken is not JwtSecurityToken)
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }

        return ValidateTokenSecurity(jwtInput, jwtHandler, paramsValidation);
    }

    public static string CreateBasic(this Guid userId, string userSecret)
    {
        var authenticationString = $"{userId}:{userSecret}";
        var base64EncodedString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
        return $"Basic {base64EncodedString}";
    }

    public static KeyValuePair<string, string> GetArgsFromBasic(this string token)
    {
        var base64EncodedAuthenticationBytes = Convert.FromBase64String(
            token.Replace("Basic ", string.Empty).Trim());

        var base64EncodedBytes = Encoding.UTF8.GetString(base64EncodedAuthenticationBytes);

        var authenticationString = base64EncodedBytes.Split(":");

        return new KeyValuePair<string, string>(authenticationString.First(), authenticationString.Last());
    }
}
