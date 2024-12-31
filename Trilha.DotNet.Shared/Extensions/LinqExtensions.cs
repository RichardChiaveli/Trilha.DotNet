namespace Trilha.DotNet.Shared.Extensions;

public static class LinqExtensions
{
    public static bool HasFilter<T>(this Expression<Func<T, bool>> predicate) =>
        !predicate.Parameters[0].Name!.Equals("f") &&
        !predicate.ToString().Equals("f => False");

    public static bool HasProperties<T>(this Expression<Func<T, object>>[] includeProperties) =>
        includeProperties is { Length: > 0 };
}