using System;

namespace BudgetManagement.Web.Common.Models
{
    public class TableState
    {
        // نام ستون برای مرتب‌سازی (همان نام پراپرتی در مدل)
        public string SortColumn { get; set; } = string.Empty;

        // جهت مرتب‌سازی: صعودی/نزولی
        public bool SortAscending { get; set; } = true;

        // صفحه فعلی (۱-مبنایی)
        public int CurrentPage { get; set; } = 1;

        // تعداد رکورد در هر صفحه
        public int PageSize { get; set; } = 10;

        // مجموع رکوردهای قابل نمایش (برای محاسبه مجموع صفحات)
        public int TotalRecords { get; set; }

        public int TotalPages =>
            PageSize <= 0 ? 1 : (int)Math.Ceiling((double)Math.Max(0, TotalRecords) / PageSize);

        public void ResetPaging() => CurrentPage = 1;
    }
}
