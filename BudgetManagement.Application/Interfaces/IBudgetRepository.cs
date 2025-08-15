using BudgetManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BudgetManagement.Application.Interfaces
{
    public interface IBudgetRepository
    {
        // متدهای تک‌به‌تک
        Task<IEnumerable<BudgetRecord>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<BudgetRecord?> GetBySubProjectCodeAsync(string subProjectCode, CancellationToken cancellationToken = default);
        Task AddAsync(BudgetRecord record, CancellationToken cancellationToken = default);
        Task UpdateAsync(BudgetRecord record, CancellationToken cancellationToken = default);
        Task DeleteAsync(BudgetRecord record, CancellationToken cancellationToken = default);
        Task DeleteBySubProjectCodeAsync(string subProjectCode, CancellationToken cancellationToken = default);

        // عملیات گروهی بهینه‌شده
        Task<Dictionary<string, BudgetRecord>> GetBySubProjectCodesAsync(
            IEnumerable<string> subProjectCodes,
            CancellationToken cancellationToken = default);

        Task AddRangeAsync(
            IEnumerable<BudgetRecord> records,
            CancellationToken cancellationToken = default);

        Task UpdateRangeAsync(
            IEnumerable<BudgetRecord> records,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
