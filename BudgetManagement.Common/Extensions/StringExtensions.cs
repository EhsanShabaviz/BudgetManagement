using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.Common.Extensions;

public static class StringExtensions
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
}
