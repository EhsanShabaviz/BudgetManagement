using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BudgetManagement.Application.DTOs.Budget
{
    public class BudgetReportFilterDto
    {
        // 📅 تاریخ قرارداد (بازه)
        public string? ContractDateFrom { get; set; }
        public string? ContractDateTo { get; set; }

        // 🧾 فیلدهای متنی
        [DisplayName("کد زیرپروژه")]
        public string? SubProjectCode { get; set; }

        [DisplayName("عنوان قرارداد")]
        public string? ContractTitle { get; set; }

        [DisplayName("شماره قرارداد")]
        public string? ContractNumber { get; set; }

        [DisplayName("پیمانکار")]
        public string? Contractor { get; set; }

        [DisplayName("کارگزار")]
        public string? Agent { get; set; }

        [DisplayName("وضعیت قرارداد")]
        public string? ContractStatus { get; set; }

        [DisplayName("معاونت اجرایی")]
        public string? ExecutiveDept { get; set; }

        [DisplayName("نحوه ارجاع کار")]
        public string? WorkReferralMethod { get; set; }

        [DisplayName("شماره تأمین")]
        public string? CreditNumber { get; set; }

        [DisplayName("ماهیت")]
        public string? Nature { get; set; }

        // 💰 فیلدهای مبلغی (همه به صورت بازه Min/Max برای اسلایدر)
        public decimal? TotalContractAmountMin { get; set; }
        public decimal? TotalContractAmountMax { get; set; }

        public decimal? InitialAmountMin { get; set; }
        public decimal? InitialAmountMax { get; set; }

        public decimal? CurrentYearTotalCreditMin { get; set; }
        public decimal? CurrentYearTotalCreditMax { get; set; }

        public decimal? TotalCreditFromStartMin { get; set; }
        public decimal? TotalCreditFromStartMax { get; set; }

        public decimal? TotalInvoicesAmountMin { get; set; }
        public decimal? TotalInvoicesAmountMax { get; set; }

        public decimal? TotalWorkProgressMin { get; set; }
        public decimal? TotalWorkProgressMax { get; set; }

        public decimal? CurrentYearInvoicesAmountMin { get; set; }
        public decimal? CurrentYearInvoicesAmountMax { get; set; }

        public decimal? AdjustmentAmountMin { get; set; }
        public decimal? AdjustmentAmountMax { get; set; }

        public decimal? MaxRequiredCreditMin { get; set; }
        public decimal? MaxRequiredCreditMax { get; set; }

        public decimal? CreditDeficitSupplyMin { get; set; }
        public decimal? CreditDeficitSupplyMax { get; set; }

        public decimal? CreditDeficitCommitmentMin { get; set; }
        public decimal? CreditDeficitCommitmentMax { get; set; }


        // 📅 سایر تاریخ‌ها (فقط مقدار تکی، بدون From/To)
        [DisplayName("تاریخ شروع")]
        public string? StartDate { get; set; }

        [DisplayName("تاریخ خاتمه")]
        public string? EndDate { get; set; }

        [DisplayName("تاریخ تمدید یافته")]
        public string? ExtendedEndDate { get; set; }
    }
}
