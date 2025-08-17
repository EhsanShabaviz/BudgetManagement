using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BudgetManagement.Application.DTOs.Budget;
using BudgetManagement.Application.Interfaces;
using BudgetManagement.Application.Validators;
using BudgetManagement.Domain.Entities;

namespace BudgetManagement.Application.UseCases
{
    public class BudgetImportUseCase : IBudgetImportUseCase
    {
        private readonly IBudgetExcelReader _excelReader;
        private readonly IBudgetRepository _repository;
        private readonly BudgetImportValidator _validator;

        public BudgetImportUseCase(
            IBudgetExcelReader excelReader,
            IBudgetRepository repository,
            BudgetImportValidator validator)
        {
            _excelReader = excelReader;
            _repository = repository;
            _validator = validator;
        }

        public async Task<ImportBudgetResultDto> PreviewAsync(Stream excelStream, CancellationToken ct = default)
        {
            var result = new ImportBudgetResultDto();

            // خواندن داده‌ها از فایل اکسل
            var parsed = (await _excelReader.ReadBudgetRecordsAsync(excelStream, ct)).ToList();
            result.TotalParsed = parsed.Count;

            // اعتبارسنجی
            var validation = await _validator.ValidateAsync(parsed, ct);
            result.Errors.AddRange(validation.Errors);

            // ✅ پر کردن لیست رکوردهای معتبر
            result.ValidRecords = validation.ValidRecords;
            result.TotalValid = validation.ValidRecords.Count;

            // در حالت Preview، هنوز هیچ چیزی در دیتابیس ذخیره نمی‌کنیم
            result.InsertedCount = 0;
            result.UpdatedCount = 0;

            return result;
        }

        public async Task<ImportBudgetResultDto> ImportAsync(Stream excelStream, CancellationToken ct = default)
        {
            var result = new ImportBudgetResultDto();

            // خواندن داده‌ها از فایل اکسل
            var parsed = (await _excelReader.ReadBudgetRecordsAsync(excelStream, ct)).ToList();
            result.TotalParsed = parsed.Count;

            // اعتبارسنجی
            var validation = await _validator.ValidateAsync(parsed, ct);
            result.Errors.AddRange(validation.Errors);
            result.ValidRecords = validation.ValidRecords;
            result.TotalValid = validation.ValidRecords.Count;

            // مرحله ذخیره‌سازی در دیتابیس
            if (validation.ValidRecords.Any())
            {
                var codes = validation.ValidRecords
                    .Select(r => r.SubProjectCode)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .ToList();

                var existingMap = await _repository.GetBySubProjectCodesAsync(codes, ct);

                var toCreate = new List<BudgetRecord>();
                var toUpdate = new List<BudgetRecord>();

                foreach (var rec in validation.ValidRecords)
                {
                    if (string.IsNullOrWhiteSpace(rec.SubProjectCode)) continue;

                    if (!existingMap.TryGetValue(rec.SubProjectCode, out var existing))
                    {
                        toCreate.Add(rec);
                    }
                    else
                    {
                        ApplyUpdate(existing, rec);
                        toUpdate.Add(existing);
                    }
                }

                if (toCreate.Count > 0)
                    await _repository.AddRangeAsync(toCreate, ct);

                if (toUpdate.Count > 0)
                    await _repository.UpdateRangeAsync(toUpdate, ct);

                await _repository.SaveChangesAsync(ct);

                result.InsertedCount = toCreate.Count;
                result.UpdatedCount = toUpdate.Count;
            }

            return result;
        }

        private static void ApplyUpdate(BudgetRecord target, BudgetRecord src)
        {
            // کلید را تغییر نده
            target.ContractTitle = src.ContractTitle;
            target.ContractNumber = src.ContractNumber;
            target.ContractDate = src.ContractDate;
            target.Contractor = src.Contractor;
            target.Agent = src.Agent;
            target.CompanyType = src.CompanyType;
            target.AgentContractNumber = src.AgentContractNumber;
            target.AgentContractDate = src.AgentContractDate;
            target.ExecutionType = src.ExecutionType;
            target.ContractStatus = src.ContractStatus;
            target.TotalContractAmount = src.TotalContractAmount;
            target.InitialAmount = src.InitialAmount;
            target.CurrentYearCashCredit = src.CurrentYearCashCredit;
            target.CurrentYearNonCashCredit = src.CurrentYearNonCashCredit;
            target.CurrentYearTotalCredit = src.CurrentYearTotalCredit;
            target.TotalCreditFromStart = src.TotalCreditFromStart;
            target.ExecutiveDept = src.ExecutiveDept;
            target.ResponsibleDept = src.ResponsibleDept;
            target.StartDate = src.StartDate;
            target.EndDate = src.EndDate;
            target.ExtendedEndDate = src.ExtendedEndDate;
            target.ActivityType = src.ActivityType;
            target.WorkReferralMethod = src.WorkReferralMethod;
            target.TotalInvoicesAmount = src.TotalInvoicesAmount;
            target.TotalWorkProgress = src.TotalWorkProgress;
            target.CurrentYearInvoicesAmount = src.CurrentYearInvoicesAmount;
            target.CreditNumber = src.CreditNumber;
            target.Nature = src.Nature;
        }


        public async Task<ImportBudgetResultDto> ImportAsync(List<BudgetRecord> records, CancellationToken ct = default)
        {
            var result = new ImportBudgetResultDto
            {
                TotalParsed = records.Count
            };

            // اعتبارسنجی رکوردها
            var validation = await _validator.ValidateAsync(records, ct);
            result.Errors.AddRange(validation.Errors);
            result.ValidRecords = validation.ValidRecords;
            result.TotalValid = validation.ValidRecords.Count;

            if (validation.ValidRecords.Any())
            {
                var codes = validation.ValidRecords
                    .Select(r => r.SubProjectCode)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .ToList();

                var existingMap = await _repository.GetBySubProjectCodesAsync(codes, ct);

                var toCreate = new List<BudgetRecord>();
                var toUpdate = new List<BudgetRecord>();

                foreach (var rec in validation.ValidRecords)
                {
                    if (string.IsNullOrWhiteSpace(rec.SubProjectCode)) continue;

                    if (!existingMap.TryGetValue(rec.SubProjectCode, out var existing))
                    {
                        toCreate.Add(rec);
                    }
                    else
                    {
                        ApplyUpdate(existing, rec);
                        toUpdate.Add(existing);
                    }
                }

                if (toCreate.Count > 0)
                    await _repository.AddRangeAsync(toCreate, ct);

                if (toUpdate.Count > 0)
                    await _repository.UpdateRangeAsync(toUpdate, ct);

                await _repository.SaveChangesAsync(ct);

                result.InsertedCount = toCreate.Count;
                result.UpdatedCount = toUpdate.Count;
            }

            return result;
        }

    }
}
