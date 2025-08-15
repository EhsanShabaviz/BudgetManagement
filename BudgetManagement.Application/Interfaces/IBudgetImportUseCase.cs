using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BudgetManagement.Application.DTOs.Budget;

namespace BudgetManagement.Application.Interfaces
{
    public interface IBudgetImportUseCase
    {
        Task<ImportBudgetResultDto> PreviewAsync(Stream excelStream, CancellationToken ct = default);
        Task<ImportBudgetResultDto> ImportAsync(Stream excelStream, CancellationToken ct = default);
    }
}
