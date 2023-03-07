namespace Trilha.DotNet.Shared.Extensions;

public static class NumberExtensions
{
    public static decimal Truncate(this decimal value, int precision)
    {
        var step = (decimal)Math.Pow(10, precision);
        var tmp = Math.Truncate(step * value);

        return tmp / step;
    }

    public static double Truncate(this double value, int precision)
    {
        var step = Math.Pow(10, precision);
        var tmp = Math.Truncate(step * value);

        return tmp / step;
    }

    public static float Truncate(this float value, int precision)
    {
        var step = (float)Math.Pow(10, precision);
        var tmp = (float)Math.Truncate(step * value);

        return tmp / step;
    }

    public static long ToLong(this string value) =>
        Convert.ToInt64(Regex.Match(value, @"\d+",
            RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1)).Value);

    public static decimal ToDecimal(this string value, string language = "pt-BR") =>
        Convert.ToDecimal(value.Replace(".",
            NumberFormatInfo.CurrentInfo.NumberDecimalSeparator), new CultureInfo(language));

    public static int? NullIfZero(this int value) => value == 0 ? null : value;

    public static decimal? NullIfZero(this decimal value) => value == 0 ? null : value;

    public static double? NullIfZero(this double value) => value == 0 ? null : value;

    public static float? NullIfZero(this float value) => value == 0 ? null : value;
}
