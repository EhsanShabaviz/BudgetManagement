using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BudgetManagement.Web.Common.Services
{
    public static class TableSortService
    {
        public static IEnumerable<T> Sort<T>(IEnumerable<T> source, string sortColumn, bool ascending)
        {
            if (source is null) return Enumerable.Empty<T>();
            if (string.IsNullOrWhiteSpace(sortColumn)) return source;

            var prop = typeof(T).GetProperty(sortColumn, BindingFlags.Public | BindingFlags.Instance);
            if (prop is null || !prop.CanRead) return source;

            // مقایسه‌کننده عمومی با پشتیبانی از null و انواع قابل مقایسه
            var comparer = Comparer<object?>.Create((a, b) =>
            {
                if (ReferenceEquals(a, b)) return 0;
                if (a is null) return 1;   // null آخر
                if (b is null) return -1;

                // اگر هر دو از یک نوع و IComparable باشند
                if (a.GetType() == b.GetType() && a is IComparable ca)
                    return ca.CompareTo(b);

                // در غیر این‌صورت به‌صورت رشته‌ای مقایسه کن
                return string.Compare(a.ToString(), b.ToString(), StringComparison.CurrentCulture);
            });

            return ascending
                ? source.OrderBy(x => prop.GetValue(x), comparer)
                : source.OrderByDescending(x => prop.GetValue(x), comparer);
        }
    }
}
