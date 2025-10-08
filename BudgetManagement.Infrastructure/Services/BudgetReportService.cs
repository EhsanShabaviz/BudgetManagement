using AutoMapper;
using BudgetManagement.Application.DTOs.Budget;
using BudgetManagement.Application.Interfaces;
using BudgetManagement.Common.Extensions;
using Microsoft.EntityFrameworkCore;


namespace BudgetManagement.Infrastructure.Services
{
    public class BudgetReportService : IBudgetReportService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;

        public BudgetReportService(IBudgetRepository budgetRepository, IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _mapper = mapper;
        }

        public async Task<List<BudgetReportDto>> GetReportAsync(BudgetReportFilterDto filter)
        {
            // مرحله ۱: کوئری پایه
            var query = _budgetRepository.GetBudgetRecordsQuery();

            // 🧾 فیلترهای متنی
            if (!string.IsNullOrWhiteSpace(filter.SubProjectCode))
                query = query.Where(x => x.SubProjectCode.Contains(filter.SubProjectCode));

            if (!string.IsNullOrWhiteSpace(filter.ContractTitle))
                query = query.Where(x => x.ContractTitle.Contains(filter.ContractTitle));

            if (!string.IsNullOrWhiteSpace(filter.ContractNumber))
                query = query.Where(x => x.ContractNumber.Contains(filter.ContractNumber));

            if (!string.IsNullOrWhiteSpace(filter.Contractor))
                query = query.Where(x => x.Contractor.Contains(filter.Contractor));

            if (!string.IsNullOrWhiteSpace(filter.Agent))
                query = query.Where(x => x.Agent.Contains(filter.Agent));

            if (!string.IsNullOrWhiteSpace(filter.ContractStatus))
                query = query.Where(x => x.ContractStatus.Contains(filter.ContractStatus));

            if (!string.IsNullOrWhiteSpace(filter.ExecutiveDept))
                query = query.Where(x => x.ExecutiveDept.Contains(filter.ExecutiveDept));

            if (!string.IsNullOrWhiteSpace(filter.WorkReferralMethod))
                query = query.Where(x => x.WorkReferralMethod.Contains(filter.WorkReferralMethod));

            if (!string.IsNullOrWhiteSpace(filter.CreditNumber))
                query = query.Where(x => x.CreditNumber.Contains(filter.CreditNumber));

            if (!string.IsNullOrWhiteSpace(filter.Nature))
                query = query.Where(x => x.Nature.Contains(filter.Nature));

            // 💰 فیلترهای مبلغی (Min/Max)
            if (filter.TotalContractAmountMin.HasValue)
                query = query.Where(x => x.TotalContractAmount >= filter.TotalContractAmountMin.Value);
            if (filter.TotalContractAmountMax.HasValue)
                query = query.Where(x => x.TotalContractAmount <= filter.TotalContractAmountMax.Value);

            if (filter.InitialAmountMin.HasValue)
                query = query.Where(x => x.InitialAmount >= filter.InitialAmountMin.Value);
            if (filter.InitialAmountMax.HasValue)
                query = query.Where(x => x.InitialAmount <= filter.InitialAmountMax.Value);

            if (filter.CurrentYearTotalCreditMin.HasValue)
                query = query.Where(x => x.CurrentYearTotalCredit >= filter.CurrentYearTotalCreditMin.Value);
            if (filter.CurrentYearTotalCreditMax.HasValue)
                query = query.Where(x => x.CurrentYearTotalCredit <= filter.CurrentYearTotalCreditMax.Value);

            if (filter.TotalCreditFromStartMin.HasValue)
                query = query.Where(x => x.TotalCreditFromStart >= filter.TotalCreditFromStartMin.Value);
            if (filter.TotalCreditFromStartMax.HasValue)
                query = query.Where(x => x.TotalCreditFromStart <= filter.TotalCreditFromStartMax.Value);

            if (filter.TotalInvoicesAmountMin.HasValue)
                query = query.Where(x => x.TotalInvoicesAmount >= filter.TotalInvoicesAmountMin.Value);
            if (filter.TotalInvoicesAmountMax.HasValue)
                query = query.Where(x => x.TotalInvoicesAmount <= filter.TotalInvoicesAmountMax.Value);

            if (filter.TotalWorkProgressMin.HasValue)
                query = query.Where(x => x.TotalWorkProgress >= filter.TotalWorkProgressMin.Value);
            if (filter.TotalWorkProgressMax.HasValue)
                query = query.Where(x => x.TotalWorkProgress <= filter.TotalWorkProgressMax.Value);

            if (filter.CurrentYearInvoicesAmountMin.HasValue)
                query = query.Where(x => x.CurrentYearInvoicesAmount >= filter.CurrentYearInvoicesAmountMin.Value);
            if (filter.CurrentYearInvoicesAmountMax.HasValue)
                query = query.Where(x => x.CurrentYearInvoicesAmount <= filter.CurrentYearInvoicesAmountMax.Value);

            if (filter.AdjustmentAmountMin.HasValue)
                query = query.Where(x => x.AdjustmentAmount >= filter.AdjustmentAmountMin.Value);
            if (filter.AdjustmentAmountMax.HasValue)
                query = query.Where(x => x.AdjustmentAmount <= filter.AdjustmentAmountMax.Value);

            if (filter.MaxRequiredCreditMin.HasValue)
                query = query.Where(x => x.MaxRequiredCredit >= filter.MaxRequiredCreditMin.Value);
            if (filter.MaxRequiredCreditMax.HasValue)
                query = query.Where(x => x.MaxRequiredCredit <= filter.MaxRequiredCreditMax.Value);

            if (filter.CreditDeficitSupplyMin.HasValue)
                query = query.Where(x => x.CreditDeficitSupply >= filter.CreditDeficitSupplyMin.Value);
            if (filter.CreditDeficitSupplyMax.HasValue)
                query = query.Where(x => x.CreditDeficitSupply <= filter.CreditDeficitSupplyMax.Value);

            if (filter.CreditDeficitCommitmentMin.HasValue)
                query = query.Where(x => x.CreditDeficitCommitment >= filter.CreditDeficitCommitmentMin.Value);
            if (filter.CreditDeficitCommitmentMax.HasValue)
                query = query.Where(x => x.CreditDeficitCommitment <= filter.CreditDeficitCommitmentMax.Value);

            // مرحله ۲: اجرای کوئری (فقط فیلترهای قابل ترجمه به SQL)
            var list = await query.ToListAsync();

            // مرحله ۳: فیلتر تاریخ قرارداد (شمسی → میلادی) در حافظه
            if (!string.IsNullOrWhiteSpace(filter.ContractDateFrom) || !string.IsNullOrWhiteSpace(filter.ContractDateTo))
            {
                DateTime? start = filter.ContractDateFrom?.PersianDigitsToLatin().TryParseShamsiDateToMiladi();
                DateTime? end = filter.ContractDateTo?.PersianDigitsToLatin().TryParseShamsiDateToMiladi();

                list = list.Where(r =>
                {
                    DateTime? contractDate = r.ContractDate?
                        .PersianDigitsToLatin()
                        .TryParseShamsiDateToMiladi();

                    bool matchStartDate = !start.HasValue || (contractDate != null && contractDate.Value.Date >= start.Value.Date);
                    bool matchEndDate = !end.HasValue || (contractDate != null && contractDate.Value.Date <= end.Value.Date);

                    return matchStartDate && matchEndDate;
                }).ToList();
            }

            // مرحله ۴: نگاشت به DTO با AutoMapper
            return _mapper.Map<List<BudgetReportDto>>(list);
        }
    }
}
