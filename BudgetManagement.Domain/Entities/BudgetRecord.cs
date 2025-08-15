using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Domain.Entities;

public class BudgetRecord
{
    [DisplayName("کد زیرپروژه")]
    [Display(Order = 0)]
    public string SubProjectCode { get; private set; } // کد زیرپروژه - کلید اصلی

    [DisplayName("عنوان قرارداد")]
    [Display(Order = 1)]
    public string? ContractTitle { get; set; }

    [DisplayName("شماره قرارداد")]
    [Display(Order = 2)]
    public string? ContractNumber { get; set; }

    [DisplayName("تاریخ قرارداد")]
    [Display(Order = 3)]
    public string? ContractDate { get; set; }

    [DisplayName("پیمانکار")]
    [Display(Order = 4)]
    public string? Contractor { get; set; }

    [DisplayName("کارگزار")]
    [Display(Order = 5)]
    public string? Agent { get; set; }

    [DisplayName("نوع شرکت")]
    [Display(Order = 6)]
    public string? CompanyType { get; set; }

    [DisplayName("شماره قرارداد کارگزاری")]
    [Display(Order = 7)]
    public string? AgentContractNumber { get; set; }

    [DisplayName("تاریخ قرارداد کارگزاری")]
    [Display(Order = 8)]
    public string? AgentContractDate { get; set; }

    [DisplayName("نحوه اجرا")]
    [Display(Order = 9)]
    public string? ExecutionType { get; set; }

    [DisplayName("وضعیت قرارداد")]
    [Display(Order = 10)]
    public string? ContractStatus { get; set; }

    [DisplayName("مبلغ کل قرارداد")]
    [Display(Order = 11)]
    public decimal? TotalContractAmount { get; set; }

    [DisplayName("مبلغ اولیه")]
    [Display(Order = 12)]
    public decimal? InitialAmount { get; set; }

    [DisplayName("تامین اعتبار نقدی سال جاری")]
    [Display(Order = 13)]
    public decimal? CurrentYearCashCredit { get; set; }

    [DisplayName("تامین اعتبار غیرنقدی سال جاری")]
    [Display(Order = 14)]
    public decimal? CurrentYearNonCashCredit { get; set; }

    [DisplayName("مبلغ کل تامین اعتبار سال جاری")]
    [Display(Order = 15)]
    public decimal? CurrentYearTotalCredit { get; set; }

    [DisplayName("کل مبلغ تامین از ابتدای پیمان")]
    [Display(Order = 16)]
    public decimal? TotalCreditFromStart { get; set; }

    [DisplayName("معاونت اجرایی")]
    [Display(Order = 17)]
    public string? ExecutiveDept { get; set; }

    [DisplayName("معاونت متولی")]
    [Display(Order = 18)]
    public string? ResponsibleDept { get; set; }

    [DisplayName("تاریخ شروع")]
    [Display(Order = 19)]
    public string? StartDate { get; set; }

    [DisplayName("تاریخ خاتمه")]
    [Display(Order = 20)]
    public string? EndDate { get; set; }

    [DisplayName("تاریخ تمدید یافته")]
    [Display(Order = 21)]
    public string? ExtendedEndDate { get; set; }

    [DisplayName("نوع فعالیت")]
    [Display(Order = 22)]
    public string? ActivityType { get; set; }

    [DisplayName("نحوه ارجاع کار")]
    [Display(Order = 23)]
    public string? WorkReferralMethod { get; set; }

    [DisplayName("مبلغ کل صورت وضعیت‌ها")]
    [Display(Order = 24)]
    public decimal? TotalInvoicesAmount { get; set; }

    [DisplayName("مبلغ کل کارکرد")]
    [Display(Order = 25)]
    public decimal? TotalWorkProgress { get; set; }

    [DisplayName("مبلغ صورت وضعیت‌ها در سال جاری")]
    [Display(Order = 26)]
    public decimal? CurrentYearInvoicesAmount { get; set; }

    [DisplayName("شماره تأمین")]
    [Display(Order = 27)]
    public string? CreditNumber { get; set; }

    [DisplayName("ماهیت")]
    [Display(Order = 28)]
    public string? Nature { get; set; }

    // Calculated Fields:
    [DisplayName("افزایش ۲۵ درصد")]
    [Display(Order = 29)]
    public decimal? Increase25Percent { get; set; }

    [DisplayName("مبلغ تعدیل")]
    [Display(Order = 30)]
    public decimal? AdjustmentAmount { get; set; }

    [DisplayName("حداکثر اعتبار موردنیاز")]
    [Display(Order = 31)]
    public decimal? MaxRequiredCredit { get; set; }

    [DisplayName("کسری اعتبار")]
    [Display(Order = 32)]
    public decimal? CreditDeficit { get; set; }

    public BudgetRecord(string subProjectCode)
    {
        if (string.IsNullOrWhiteSpace(subProjectCode))
            throw new ArgumentException("SubProjectCode is required");

        SubProjectCode = subProjectCode;
    }
}
