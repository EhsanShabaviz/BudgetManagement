using BudgetManagement.Application.DTOs;
using BudgetManagement.Application.Interfaces;
using BudgetManagement.Domain.Entities;
using BudgetManagement.Infrastructure.Persistence;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BudgetManagement.Infrastructure.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly BudgetManagementDbContext _db;

        public BudgetRepository(BudgetManagementDbContext db)
        {
            _db = db;
        }

        // 📌 فقط خواندن → بدون Tracking
        public async Task<IEnumerable<BudgetRecord>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.BudgetRecords
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // 📌 برای ویرایش → Tracking فعال
        public async Task<BudgetRecord?> GetBySubProjectCodeAsync(string subProjectCode, CancellationToken cancellationToken = default)
        {
            return await _db.BudgetRecords
                .FirstOrDefaultAsync(r => r.SubProjectCode == subProjectCode, cancellationToken);
        }

        public async Task<List<string>> GetExecutiveDeptAsync(CancellationToken cancellationToken = default)
        {
            return await _db.BudgetRecords
                .Select(r => r.ExecutiveDept)
                .Where(d => !string.IsNullOrEmpty(d))         // remove null or empty
                .Distinct()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<string>> GetContractStatusAsync(CancellationToken cancellationToken = default)
        {
            return await _db.BudgetRecords
                .Select(r => r.ContractStatus)
                .Where(d => !string.IsNullOrEmpty(d))         // remove null or empty
                .Distinct()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<string>> GetWorkReferralMethodAsync(CancellationToken cancellationToken = default)
        {
            return await _db.BudgetRecords
                .Select(r => r.WorkReferralMethod)
                .Where(d => !string.IsNullOrEmpty(d))         // remove null or empty
                .Distinct()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<string>> GetNatureAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Natures
                .Select(r => r.Name)
                .Where(d => !string.IsNullOrEmpty(d))         // remove null or empty
                .Distinct()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(BudgetRecord record, CancellationToken cancellationToken = default)
        {
            await _db.BudgetRecords.AddAsync(record, cancellationToken);
        }

        public Task UpdateAsync(BudgetRecord record, CancellationToken cancellationToken = default)
        {
            _db.BudgetRecords.Update(record);
            return Task.CompletedTask;
        }

        public async Task UpdateBySubProjectCodeAsync(string subProjectCode, BudgetRecord updatedEntity, CancellationToken cancellationToken = default)
        {
            if (subProjectCode != updatedEntity.SubProjectCode)
                return;

            _db.BudgetRecords.Update(updatedEntity);
            await Task.CompletedTask;
        }

        public Task DeleteAsync(BudgetRecord record, CancellationToken cancellationToken = default)
        {
            _db.BudgetRecords.Remove(record);
            return Task.CompletedTask;
        }

        public async Task DeleteBySubProjectCodeAsync(string subProjectCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(subProjectCode))
                return;

            var entity = await GetBySubProjectCodeAsync(subProjectCode, cancellationToken);
            if (entity != null)
            {
                _db.BudgetRecords.Remove(entity);
            }
        }

        // 📌 فقط خواندن → بدون Tracking
        public async Task<Dictionary<string, BudgetRecord>> GetBySubProjectCodesAsync(
            IEnumerable<string> subProjectCodes,
            CancellationToken cancellationToken = default)
        {
            return await _db.BudgetRecords
                .AsNoTracking()
                .Where(r => subProjectCodes.Contains(r.SubProjectCode))
                .ToDictionaryAsync(r => r.SubProjectCode, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<BudgetRecord> records, CancellationToken cancellationToken = default)
        {
            await _db.BudgetRecords.AddRangeAsync(records, cancellationToken);
        }

        public Task UpdateRangeAsync(IEnumerable<BudgetRecord> records, CancellationToken cancellationToken = default)
        {
            _db.BudgetRecords.UpdateRange(records);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }


        public async Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync(CancellationToken cancellationToken = default)
        {
            return await _db.AuditLogs
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<AuditLogDto>> GetAllAuditLogsWithUserAsync(CancellationToken cancellationToken = default)
        {
            return await _db.AuditLogs
                .Join(_db.Users,
                      log => log.UserId,
                      user => user.Id,
                      (log, user) => new AuditLogDto
                      {
                          Id = log.Id,
                          UserId = log.UserId,
                          UserName = user.UserName ?? "Unknown", // 👈 گرفتن نام کاربری
                          ActionType = log.ActionType,
                          Description = log.Description,
                          DateTime = log.DateTime,
                          IPAddress = log.IPAddress,
                          BrowserInfo = log.BrowserInfo
                      })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }


        public async Task<List<string>> GetActionTypeAsync(CancellationToken cancellationToken = default)
        {
            return await _db.AuditLogs
                .Select(r => r.ActionType)
                .Distinct()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public IQueryable<BudgetRecord> GetBudgetRecordsQuery()
        {
            return _db.BudgetRecords.AsQueryable();
        }
    }
}
