using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Application.DTOs.Budget
{
    public class BudgetReportDto
    {
        [DisplayName("کد زیرپروژه")]
        public string SubProjectCode { get; set; } = string.Empty;

        [StringLength(300), DisplayName("عنوان قرارداد")]
        public string? ContractTitle { get; set; }

        [StringLength(50), DisplayName("شماره قرارداد")]
        public string? ContractNumber { get; set; }

        [StringLength(50), DisplayName("تاریخ قرارداد")]
        public string? ContractDate { get; set; }

        [StringLength(250), DisplayName("پیمانکار")]
        public string? Contractor { get; set; }

        [StringLength(250), DisplayName("کارگزار")]
        public string? Agent { get; set; }

        [StringLength(250), DisplayName("وضعیت قرارداد")]
        public string? ContractStatus { get; set; }

        [DisplayName("مبلغ کل قرارداد")]
        public decimal? TotalContractAmount { get; set; }

        [DisplayName("مبلغ اولیه")]
        public decimal? InitialAmount { get; set; }

        [DisplayName("مبلغ کل تأمین اعتبار سال جاری")]
        public decimal? CurrentYearTotalCredit { get; set; }

        [DisplayName("کل مبلغ تأمین از ابتدای پیمان")]
        public decimal? TotalCreditFromStart { get; set; }

        [StringLength(250), DisplayName("معاونت اجرایی")]
        public string? ExecutiveDept { get; set; }

        [StringLength(50), DisplayName("تاریخ شروع")]
        public string? StartDate { get; set; }

        [StringLength(50), DisplayName("تاریخ خاتمه")]
        public string? EndDate { get; set; }

        [StringLength(50), DisplayName("تاریخ تمدید یافته")]
        public string? ExtendedEndDate { get; set; }

        [StringLength(250), DisplayName("نحوه ارجاع کار")]
        public string? WorkReferralMethod { get; set; }

        [DisplayName("مبلغ کل صورت وضعیت‌ها")]
        public decimal? TotalInvoicesAmount { get; set; }

        [DisplayName("مبلغ کل کارکرد")]
        public decimal? TotalWorkProgress { get; set; }

        [DisplayName("مبلغ صورت وضعیت‌ها در سال جاری")]
        public decimal? CurrentYearInvoicesAmount { get; set; }

        [StringLength(250), DisplayName("شماره تأمین")]
        public string? CreditNumber { get; set; }

        [StringLength(250), DisplayName("ماهیت")]
        public string? Nature { get; set; }

        [DisplayName("مبلغ تعدیل")]
        public decimal? AdjustmentAmount { get; set; }

        [DisplayName("حداکثر اعتبار موردنیاز")]
        public decimal? MaxRequiredCredit { get; set; }

        [DisplayName("کسری اعتبار تأمین")]
        public decimal? CreditDeficitSupply { get; set; }

        [DisplayName("کسری اعتبار تعهد")]
        public decimal? CreditDeficitCommitment { get; set; }
    }
}
