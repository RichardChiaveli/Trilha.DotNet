namespace Trilha.DotNet.Shared.Utils;

public static class DateTimeUtils
{
    public const string DateFormatEnUs = "yyyy-MM-dd";
    public const string DateFormatPtBr = "dd-MM-yyyy";
    public const string DateTimeFormatEnUs = "yyyy-MM-dd HH:mm:ss.fff";
    public const string DateTimeFormatPtBr = "dd-MM-yyyy HH:mm:ss.fff";
    public const string TimeFormat = "HH:mm:ss.fff";

    public static Dictionary<string, int> AllDays
    {
        get
        {
            var days = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
            return Enumerable.Range(0, 7).ToDictionary(id => string.Concat(char.ToUpper(days[id][0]), days[id][1..]), id => id + 1);
        }
    }
}
