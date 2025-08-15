using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BudgetManagement.Domain.Entities;
using BudgetManagement.Application.Interfaces;


namespace BudgetManagement.Application.Validators
{
    public class BudgetImportValidationResult
    {
        public List<string> Errors { get; set; } = new();
        public List<BudgetRecord> ValidRecords { get; set; } = new();
        public bool HasErrors => Errors.Any();
    }

    public class BudgetImportValidator
    {
        /// <summary>
        /// Validate budget records from Excel file for duplicate keys and required fields.
        /// </summary>
        public Task<BudgetImportValidationResult> ValidateAsync(IEnumerable<BudgetRecord> records)
        {
            var result = new BudgetImportValidationResult();
            var recordList = records.ToList();

            // بررسی تکراری بودن SubProjectCode در فایل
            var duplicateInFile = recordList
                .GroupBy(r => r.SubProjectCode)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateInFile.Any())
            {
                result.Errors.AddRange(
                    duplicateInFile.Select(code => $"Duplicate SubProjectCode in file: {code}")
                );
            }

            // بررسی فیلدهای ضروری
            foreach (var record in recordList)
            {
                if (string.IsNullOrWhiteSpace(record.SubProjectCode))
                    result.Errors.Add("SubProjectCode is required.");

                if (string.IsNullOrWhiteSpace(record.ContractTitle))
                    result.Errors.Add($"ContractTitle is required for SubProjectCode: {record.SubProjectCode}");
            }

            // فقط رکوردهای بدون خطا را به ValidRecords اضافه می‌کنیم
            if (!result.HasErrors)
            {
                result.ValidRecords.AddRange(recordList);
            }

            return Task.FromResult(result);
        }
    }
}
