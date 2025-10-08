using BudgetManagement.Application.DTOs.Budget;

namespace BudgetManagement.Application.Interfaces
{
    public interface IBudgetReportService
    {
        /// <summary>
        /// دریافت گزارش بودجه بر اساس شرایط فیلتر
        /// </summary>
        /// <param name="filter">شرایط جستجو شامل بازه تاریخ و مبالغ و سایر فیلدها</param>
        /// <returns>لیستی از BudgetReportDto به عنوان خروجی گزارش</returns>
        Task<List<BudgetReportDto>> GetReportAsync(BudgetReportFilterDto filter);
    }
}
