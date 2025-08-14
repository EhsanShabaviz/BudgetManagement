using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManagement.Domain.Entities;
using BudgetManagement.Application.Interfaces;

namespace BudgetManagement.Application.Services
{
    public class BudgetImportResult
    {
        public int InsertedCount { get; set; }
        public int UpdatedCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class BudgetImportValidator
    {
        private readonly IBudgetRepository _repository;

        public BudgetImportValidator(IBudgetRepository repository)
        {
            _repository = repository;
        }

        public async Task<BudgetImportResult> ValidateAndProcessAsync(IEnumerable<BudgetRecord> records)
        {
            var result = new BudgetImportResult();

            // پیدا کردن کدهای تکراری داخل فایل اکسل
            var duplicateInFile = records
                .GroupBy(r => r.SubProjectCode)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateInFile.Any())
            {
                result.Errors.AddRange(duplicateInFile.Select(code => $"Duplicate SubProjectCode in file: {code}"));
                return result;
            }

            // گرفتن همه SubProjectCode های موجود در دیتابیس
            var existingRecords = await _repository.GetAllAsync();
            var existingCodes = existingRecords.Select(r => r.SubProjectCode).ToHashSet();

            foreach (var record in records)
            {
                if (existingCodes.Contains(record.SubProjectCode))
                {
                    await _repository.UpdateAsync(record);
                    result.UpdatedCount++;
                }
                else
                {
                    await _repository.AddAsync(record);
                    result.InsertedCount++;
                }
            }

            return result;
        }
    }
}
