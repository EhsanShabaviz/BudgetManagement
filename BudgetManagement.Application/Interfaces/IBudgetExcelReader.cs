using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BudgetManagement.Domain.Entities;

namespace BudgetManagement.Application.Interfaces
{
    // Port در لایه Application؛ پیاده‌سازی در Infrastructure انجام می‌شود
    public interface IBudgetExcelReader
    {
        Task<IEnumerable<BudgetRecord>> ReadBudgetRecordsAsync(
            Stream excelStream,
            CancellationToken cancellationToken = default);
    }
}
