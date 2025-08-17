using BudgetManagement.Application.Interfaces;
using BudgetManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.Application.UseCases
{
    public class BudgetCalculationUseCase : IBudgetCalculationUseCase
    {
        public Task CalculateAsync(List<BudgetRecord> records)
        {
            /*foreach (var record in records)
            {
                if (record.TotalInvoicesAmount.HasValue && record.TotalWorkProgress.HasValue)
                    record.AdjustmentAmount = record.TotalInvoicesAmount.Value - record.TotalWorkProgress.Value;

                if (record.TotalContractAmount.HasValue && record.AdjustmentAmount.HasValue)
                    record.MaxRequiredCredit = record.TotalContractAmount.Value * 1.1m + record.AdjustmentAmount.Value;

                if (record.MaxRequiredCredit.HasValue && record.TotalCreditFromStart.HasValue)
                    record.CreditDeficit = record.MaxRequiredCredit.Value - record.TotalCreditFromStart.Value;
            }*/

            foreach (var r in records)
            {
                r.AdjustmentAmount = (r.TotalInvoicesAmount ?? 0) - (r.TotalWorkProgress ?? 0);
                r.MaxRequiredCredit = (r.TotalContractAmount ?? 0) * 1.1m + (r.AdjustmentAmount ?? 0);
                r.CreditDeficit = (r.MaxRequiredCredit ?? 0) - (r.TotalCreditFromStart ?? 0);
            }

            return Task.CompletedTask;
        }
    }

}
