using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.Domain.Entities;

public class BudgetRecord
{
    public string SubProjectCode { get; private set; } // کد زیرپروژه - کلید اصلی
    public string? ContractTitle { get; set; }
    public string? ContractNumber { get; set; }
    public string? ContractDate { get; set; }
    public string? Contractor { get; set; }
    public string? Agent { get; set; }
    public string? CompanyType { get; set; }
    public string? AgentContractNumber { get; set; }
    public string? AgentContractDate { get; set; }
    public string? ExecutionType { get; set; }
    public string? ContractStatus { get; set; }
    public decimal? TotalContractAmount { get; set; }
    public decimal? InitialAmount { get; set; }
    public decimal? CurrentYearCashCredit { get; set; }
    public decimal? CurrentYearNonCashCredit { get; set; }
    public decimal? CurrentYearTotalCredit { get; set; }
    public decimal? TotalCreditFromStart { get; set; }
    public string? ExecutiveDept { get; set; }
    public string? ResponsibleDept { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? ExtendedEndDate { get; set; }
    public string? ActivityType { get; set; }
    public string? WorkReferralMethod { get; set; }
    public decimal? TotalInvoicesAmount { get; set; }
    public decimal? TotalWorkProgress { get; set; }
    public decimal? CurrentYearInvoicesAmount { get; set; }
    public string? CreditNumber { get; set; }
    public string? Nature { get; set; }

    // Calculated Fields:
    public decimal? Increase25Percent { get; set; }
    public decimal? AdjustmentAmount { get; set; }
    public decimal? MaxRequiredCredit { get; set; }
    public decimal? CreditDeficit { get; set; }

    public BudgetRecord(string subProjectCode)
    {
        if (string.IsNullOrWhiteSpace(subProjectCode))
            throw new ArgumentException("SubProjectCode is required");

        SubProjectCode = subProjectCode;
    }
}

