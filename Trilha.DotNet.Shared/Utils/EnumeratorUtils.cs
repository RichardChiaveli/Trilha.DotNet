namespace Trilha.DotNet.Shared.Utils;

public static class EnumeratorUtils<T> where T : Enum
{
    public static IEnumerable<string> Describe()
    {
        var type = typeof(T);

        foreach (var eEnum in Enum.GetValues(type).Cast<Enum>())
        {
            var value = Convert.ChangeType(eEnum, Enum.GetUnderlyingType(eEnum.GetType()));
            yield return $"{value} - {eEnum.GetDescriptionFromAttribute()}";
        }
    }

    public static IEnumerable<T> All() => Enum.GetValues(typeof(T)).Cast<T>();

    public static IEnumerable<string> GetAllDescriptionsFromAttribute()
    {
        var attributes =
            typeof(T).GetMembers().SelectMany(i =>
                i.GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>());

        return attributes.Select(x => x.Description);
    }
}
