namespace Trilha.DotNet.Shared.Extensions;

public static class ReflectionExtensions
{
    public static string PrintMethodParams(this MethodBase? method, params object[] values)
    {
        if (method == null)
            return string.Empty;

        var parms = method.GetParameters();
        var namevalues = new object[2 * parms.Length];

        var msg = new StringBuilder();
        msg.Append($"{method.Name}(");

        if (values.Length > 0)
        {
            for (int i = 0, j = 0; i < parms.Length; i++, j += 2)
            {
                msg.Append("{" + j + "}={" + (j + 1) + "}" + (parms.Length != i + 1 ? ", " : ")"));
                namevalues[j] = parms[i].Name ?? string.Empty;

                if (i < values.Length) namevalues[j + 1] = values[i].Stringify();
            }
        }
        else
        {
            msg.Append(')');
        }

        var classe = method.ReflectedType?.Name;
        return $"{classe}.{string.Format(msg.ToString(), namevalues)}";
    }

    public static PropertyInfo GetAttributeFromAnnotattion<T>(this Type type, string columnName, string propName = "Name") where T : Attribute
    {
        return type.GetProperties().First(prop =>
            {
                var attribute = prop.GetCustomAttribute(typeof(T), false) as T;
                var value = attribute?.GetType().GetProperty(propName)?.GetValue(attribute, null)?.ToString() ?? prop.Name;

                return string.Equals(value, columnName, StringComparison.CurrentCultureIgnoreCase);
            });
    }

    public static IEnumerable<PropertyInfo> GetSystemProperties(this object argument) =>
        argument.GetType().GetProperties()
            .Where(i => (i.PropertyType.FullName ?? string.Empty).Contains("System"));

    public static void CopyValues<T>(this T target, T source)
    {
        var t = typeof(T);

        var properties = t.GetProperties().Where(prop => prop is { CanRead: true, CanWrite: true });

        foreach (var prop in properties)
        {
            var value = prop.GetValue(source, null);
            if (value != null)
                prop.SetValue(target, value, null);
        }
    }
}
