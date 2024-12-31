namespace Trilha.DotNet.Shared.Contracts;

public sealed record ResultError(HttpStatusCode HttpStatusCode = HttpStatusCode.BadRequest, params string[] Messages);