namespace Trilha.DotNet.Shared.Extensions;

public static class EnumExtensions
{
    public static string GetDescriptionFromAttribute(this Enum enumerator)
    {
        var fi = enumerator.GetType().GetField(enumerator.ToString());

        if (fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any())
            return attributes.FirstOrDefault()?.Description ?? string.Empty;

        return enumerator.ToString();
    }

    public static T Deserialize<T>(this string status) where T : Enum
    {
        foreach (var field in typeof(T).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute && attribute.Description == status)
            {
                return (T)field.GetValue(null)!;
            }

            if (field.Name == status)
                return (T)field.GetValue(null)!;
        }

        throw new ArgumentException("Not found.", nameof(status));
    }

    public static bool IsValid<T>(this int enumInt) => Enum.IsDefined(typeof(T), enumInt);
}
