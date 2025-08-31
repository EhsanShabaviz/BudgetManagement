using System.Globalization;
using System.Text;

namespace BudgetManagement.Web.Extensions;

public static class PersianStringExtensions
{
    /// <summary>
    /// Converts Persian/Arabic digits in the input string into standard Latin (0-9) digits.
    /// This is useful when working with user input from PersianDatePickers or text boxes,
    /// for When use this Option --> DigitType="DigitType.Persian"
    /// since .NET parsing methods like DateTime.TryParse only recognize Latin digits.
    /// Example: "۱۴۰۳/۰۶/۰۷" → "1403/06/07"
    /// <returns>A normalized string with only Latin digits.</returns>
    /// /// </summary>
    public static string PersianDigitsToLatin(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        //Version 01
        /*return input
            .Replace("۰", "0")
            .Replace("۱", "1")
            .Replace("۲", "2")
            .Replace("۳", "3")
            .Replace("۴", "4")
            .Replace("۵", "5")
            .Replace("۶", "6")
            .Replace("۷", "7")
            .Replace("۸", "8")
            .Replace("۹", "9")
            // In case Arabic-Indic digits are also used:
            .Replace("٠", "0")
            .Replace("١", "1")
            .Replace("٢", "2")
            .Replace("٣", "3")
            .Replace("٤", "4")
            .Replace("٥", "5")
            .Replace("٦", "6")
            .Replace("٧", "7")
            .Replace("٨", "8")
            .Replace("٩", "9");*/

        //Version 02
        #region  
        var sb = new StringBuilder(input.Length);

        foreach (var ch in input)
        {
            sb.Append(ch switch
            {
                // Persian digits (U+06F0 to U+06F9)
                '۰' => '0',
                '۱' => '1',
                '۲' => '2',
                '۳' => '3',
                '۴' => '4',
                '۵' => '5',
                '۶' => '6',
                '۷' => '7',
                '۸' => '8',
                '۹' => '9',

                // Arabic-Indic digits (U+0660 to U+0669)
                '٠' => '0',
                '١' => '1',
                '٢' => '2',
                '٣' => '3',
                '٤' => '4',
                '٥' => '5',
                '٦' => '6',
                '٧' => '7',
                '٨' => '8',
                '٩' => '9',

                _ => ch // keep everything else as-is
            });
        }

        return sb.ToString();
        #endregion
    }

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
        if (DateTime.TryParse(dateStr, out var gregorianDate))
            return gregorianDate;

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

}

