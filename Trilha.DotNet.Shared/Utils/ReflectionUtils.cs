namespace Trilha.DotNet.Shared.Utils;

public static class ReflectionUtils
{
    public static bool IsList<T>()
        => typeof(T).GetInterface(nameof(IEnumerable)) != null || typeof(T).GetInterface(nameof(IList)) != null;
}