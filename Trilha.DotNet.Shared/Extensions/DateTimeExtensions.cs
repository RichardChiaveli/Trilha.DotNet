namespace Trilha.DotNet.Shared.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToFirstDate(this DateTime value) => new(value.Year, value.Month, 1, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime ToLastDate(this DateTime value) => new(value.Year, value.Month,
        DateTime.DaysInMonth(value.Year, value.Month), 0, 0, 0, DateTimeKind.Utc);

    public static long ToUnixEpochDate(this DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds);

    public static bool Validate(this string format, string intialDate, string finalDate)
    {
        var isValid =
            DateTime.TryParseExact(intialDate, format,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt1);

        if (!isValid)
            return false;

        isValid =
            DateTime.TryParseExact(finalDate, format,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt2);

        return isValid && dt2 >= dt1;
    }

    public static bool ValidateDayLimit(this int limit, DateTime initialDate, DateTime finalDate)
        => (finalDate - initialDate).Days <= limit;

    public static string ToDayName(this int dayValue)
    {
        var day = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[dayValue];
        return string.Concat(char.ToUpper(day[0]), day[1..]);
    }

    public static int ToDayValue(this string dayName)
    {
        var days = CultureInfo.CurrentCulture.DateTimeFormat.DayNames.Select((val, index) => new
        {
            Value = val,
            Index = index
        }).ToList();

        var result = days.First(i => i.Value == dayName).Index;
        result += 1;

        return result;
    }
}
