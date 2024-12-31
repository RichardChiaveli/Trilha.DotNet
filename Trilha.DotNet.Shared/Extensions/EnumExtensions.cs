namespace Trilha.DotNet.Shared.Extensions;

public static class EnumExtensions
{
    public static string GetDescriptionFromAttribute(this Enum enumerator)
    {
        var fi = enumerator.GetType().GetField(enumerator.ToString());

        return fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] { Length: > 0 } attributes
            ? attributes.FirstOrDefault()?.Description ?? string.Empty
            : enumerator.ToString();
    }

    public static T Deserialize<T>(this string status) where T : Enum
    {
        var enumType = typeof(T);
        var fields = enumType.GetFields();

        foreach (var field in fields)
        {
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute && attribute.Description == status)
            {
                return (T)field.GetValue(null)!;
            }
        }

        return (T)Enum.Parse(enumType, status, ignoreCase: true);
    }

    public static bool IsValid<T>(this int enumInt) => Enum.IsDefined(typeof(T), enumInt);
}
