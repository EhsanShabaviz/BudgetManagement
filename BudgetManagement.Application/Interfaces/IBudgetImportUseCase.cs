using BudgetManagement.Application.DTOs.Budget;
using BudgetManagement.Domain.Entities;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BudgetManagement.Application.Interfaces
{
    public interface IBudgetImportUseCase
    {
        Task<ImportBudgetResultDto> PreviewAsync(Stream excelStream, CancellationToken ct = default);
        Task<ImportBudgetResultDto> ImportAsync(Stream excelStream, CancellationToken ct = default);

        // ✅ امضای جدید برای بارگذاری داده‌های محاسبه‌شده
        Task<ImportBudgetResultDto> ImportAsync(List<BudgetRecord> records, CancellationToken ct = default);
    }
}
