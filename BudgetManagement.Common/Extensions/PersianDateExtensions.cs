using System.Globalization;
using System.Text;

namespace BudgetManagement.Common.Extensions;

public static class PersianDateExtensions
{
    /// <summary>
    /// Attempts to parse the current string as a date in either Gregorian (Miladi) or Persian (Shamsi) format.
    /// If the string represents a valid Gregorian date, it is returned as-is.
    /// If the string represents a valid Persian date (in formats like yyyy/MM/dd or yyyy-MM-dd),
    /// it is converted to its equivalent Gregorian <see cref="DateTime"/> using <see cref="PersianCalendar"/>.
    /// Returns <c>null</c> if the input is null, empty, or cannot be parsed as a valid date.
    /// </summary>
    /// <param name="dateStr">
    /// The date string to parse. Can be in Gregorian or Persian date format.
    /// </param>
    /// <returns>
    /// A <see cref="DateTime"/> in Gregorian calendar if parsing succeeds; otherwise, <c>null</c>.
    /// </returns>
    public static DateTime? TryParseShamsiDateToMiladi(this string? dateStr)
    {
        // Return null if the input is null, empty, or whitespace
        if (string.IsNullOrWhiteSpace(dateStr))
            return null;

        // First, try to parse the input directly as a Gregorian (Miladi) date
        /*if (DateTime.TryParse(dateStr, out var gregorianDate))
            return gregorianDate;*/

        // Try to parse the input as a Shamsi (Persian) date and convert it to Gregorian
        try
        {
            // Split the date string by '/' or '-'
            var parts = dateStr.Split(new[] { '/', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3) return null;

            // Safely parse year, month, and day(Version 01)
            /*int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int day = int.Parse(parts[2]);*/

            // Safely parse year, month, and day(Version 02)
            if (!int.TryParse(parts[0], out int year) ||
                !int.TryParse(parts[1], out int month) ||
                !int.TryParse(parts[2], out int day))
                return null;

            // Validate month range (1–12)
            if (month is < 1 or > 12) return null;

            // Determine the maximum valid day for the given month in the Persian calendar
            var pc = new PersianCalendar();
            int maxDay = month <= 6 ? 31 : (month <= 11 ? 30 : 29);
            if (month == 12 && pc.IsLeapYear(year)) maxDay = 30;

            // Validate day range
            if (day < 1 || day > maxDay) return null;

            // Convert the Persian date to Gregorian DateTime
            return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
        }
        catch
        {
            // Return null if any unexpected error occurs during parsing/conversion
            return null;
        }
    }

    public static string ToShamsiDate(this DateTime dateTime)
    {
        var pc = new PersianCalendar();
        return $"{pc.GetYear(dateTime):0000}/{pc.GetMonth(dateTime):00}/{pc.GetDayOfMonth(dateTime):00} ";
    }

    public static string ToShamsiDateTime(this DateTime dateTime)
    {
        var pc = new PersianCalendar();
        return $"{pc.GetYear(dateTime):0000}/{pc.GetMonth(dateTime):00}/{pc.GetDayOfMonth(dateTime):00} " +
               $"{dateTime:HH:mm:ss}";
    }

}

