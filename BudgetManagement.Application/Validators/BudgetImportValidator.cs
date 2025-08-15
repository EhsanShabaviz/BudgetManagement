using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BudgetManagement.Application.DTOs.Budget;
using BudgetManagement.Domain.Entities;

namespace BudgetManagement.Application.Validators
{
    public class BudgetImportValidator
    {
        public Task<BudgetValidationResultDto> ValidateAsync(
            IEnumerable<BudgetRecord> records,
            CancellationToken cancellationToken = default)
        {
            var result = new BudgetValidationResultDto();
            var list = records?.ToList() ?? new List<BudgetRecord>();
            if (list.Count == 0)
            {
                result.Errors.Add("هیچ رکوردی در فایل یافت نشد.");
                return Task.FromResult(result);
            }

            // 1) کد زیرپروژه الزامی
            var missingCodes = list.Where(r => string.IsNullOrWhiteSpace(r.SubProjectCode)).ToList();
            if (missingCodes.Count > 0)
                result.Errors.Add($"تعداد {missingCodes.Count} رکورد بدون SubProjectCode است.");

            // 2) تکرار کد زیرپروژه در خود فایل
            var dupGroups = list.Where(r => !string.IsNullOrWhiteSpace(r.SubProjectCode))
                                .GroupBy(r => r.SubProjectCode)
                                .Where(g => g.Count() > 1)
                                .ToList();
            if (dupGroups.Count > 0)
            {
                var dupCodes = string.Join(", ", dupGroups.Select(g => $"{g.Key} x{g.Count()}"));
                result.Errors.Add($"کدهای تکراری در فایل: {dupCodes}");
            }

            // 3) اعتبار عددی ساده (منفی نباشند)
            foreach (var r in list)
            {
                if (r.TotalContractAmount < 0 ||
                    r.InitialAmount < 0 ||
                    r.CurrentYearCashCredit < 0 ||
                    r.CurrentYearNonCashCredit < 0 ||
                    r.CurrentYearTotalCredit < 0 ||
                    r.TotalCreditFromStart < 0 ||
                    r.TotalInvoicesAmount < 0 ||
                    r.CurrentYearInvoicesAmount < 0)
                {
                    result.Errors.Add($"مقادیر منفی برای SubProjectCode={r.SubProjectCode} مجاز نیست.");
                }
            }

            // رکوردهای معتبر = آنهایی که کد دارند و در گروه تکراری نیستند و خطای عددی ندارند
            var badCodes = new HashSet<string>(missingCodes.Select(r => r.SubProjectCode ?? string.Empty));
            foreach (var g in dupGroups) badCodes.Add(g.Key);

            // اگر خطاهای عددی داریم، آنها را هم حذف کنیم
            var numericErrorCodes = list
                .Where(r => r.TotalContractAmount < 0 ||
                            r.InitialAmount < 0 ||
                            r.CurrentYearCashCredit < 0 ||
                            r.CurrentYearNonCashCredit < 0 ||
                            r.CurrentYearTotalCredit < 0 ||
                            r.TotalCreditFromStart < 0 ||
                            r.TotalInvoicesAmount < 0 ||
                            r.CurrentYearInvoicesAmount < 0)
                .Select(r => r.SubProjectCode ?? string.Empty);

            foreach (var c in numericErrorCodes)
                badCodes.Add(c);

            result.ValidRecords = list
                .Where(r => !string.IsNullOrWhiteSpace(r.SubProjectCode))
                .Where(r => !badCodes.Contains(r.SubProjectCode!))
                .ToList();

            return Task.FromResult(result);
        }
    }
}
