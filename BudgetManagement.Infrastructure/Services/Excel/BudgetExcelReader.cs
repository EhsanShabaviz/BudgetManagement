using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BudgetManagement.Application.Interfaces;
using BudgetManagement.Domain.Entities;
using ClosedXML.Excel;

namespace BudgetManagement.Infrastructure.Services.Excel
{
    public class BudgetExcelReader : IBudgetExcelReader
    {
        // معادل‌های فارسی برای هدرها
        private static readonly Dictionary<string, string[]> HeaderSynonyms = new(StringComparer.OrdinalIgnoreCase)
        {
            ["SubProjectCode"] = new[] { "کد زیرپروژه", "کدزیرپروژه" },
            ["ContractTitle"] = new[] { "عنوان قرارداد" },
            ["ContractNumber"] = new[] { "شماره قرارداد" },
            ["ContractDate"] = new[] { "تاریخ قرارداد" },
            ["Contractor"] = new[] { "پیمانکار" },
            ["Agent"] = new[] { "کارگزار" },
            ["CompanyType"] = new[] { "نوع شرکت" },
            ["AgentContractNumber"] = new[] { "شماره قرارداد کارگزاری" },
            ["AgentContractDate"] = new[] { "تاریخ قرارداد کارگزاری" },
            ["ExecutionType"] = new[] { "نحوه اجرا" },
            ["ContractStatus"] = new[] { "وضعیت قرارداد" },
            ["TotalContractAmount"] = new[] { "مبلغ کل قرارداد" },
            ["InitialAmount"] = new[] { "مبلغ اولیه" },
            ["CurrentYearCashCredit"] = new[] { "تامین اعتبار نقدی سال جاری" },
            ["CurrentYearNonCashCredit"] = new[] { "تامین اعتبار غیرنقدی سال جاری" },
            ["CurrentYearTotalCredit"] = new[] { "مبلغ کل تامین اعتبار سال جاری" },
            ["TotalCreditFromStart"] = new[] { "کل مبلغ تامین از ابتدای پیمان" },
            ["ExecutiveDept"] = new[] { "معاونت اجرایی" },
            ["ResponsibleDept"] = new[] { "معاونت متولی" },
            ["StartDate"] = new[] { "تاریخ شروع" },
            ["EndDate"] = new[] { "تاریخ خاتمه" },
            ["ExtendedEndDate"] = new[] { "تاریخ تمدید یافته" },
            ["ActivityType"] = new[] { "نوع فعالیت" },
            ["WorkReferralMethod"] = new[] { "نحوه ارجاع کار" },
            ["TotalInvoicesAmount"] = new[] { "مبلغ کل صورت وضعیت ها", "مبلغ کل صورت وضعیت‌ها" },
            ["TotalWorkProgress"] = new[] { "مبلغ کل کارکرد", "درصد پیشرفت" },
            ["CurrentYearInvoicesAmount"] = new[] { "مبلغ صورت وضعیت ها در سال جاری", "مبلغ صورت وضعیت‌ها در سال جاری" },
            ["CreditNumber"] = new[] { "شماره تامین", "شماره تأمین" },
            ["Nature"] = new[] { "ماهیت" },
        };

        public Task<IEnumerable<BudgetRecord>> ReadBudgetRecordsAsync(
            Stream excelStream,
            CancellationToken cancellationToken = default)
        {
            if (excelStream == null) throw new ArgumentNullException(nameof(excelStream));
            if (excelStream.CanSeek) excelStream.Position = 0;

            var records = new List<BudgetRecord>();

            using var workbook = new XLWorkbook(excelStream);
            var ws = workbook.Worksheets.FirstOrDefault()
                     ?? throw new InvalidOperationException("Worksheet not found.");

            var firstRow = ws.FirstRowUsed();
            if (firstRow is null) return Task.FromResult<IEnumerable<BudgetRecord>>(records);

            // ساخت نقشه هدرها با نرمال‌سازی یونی‌کد
            var headerMap = BuildHeaderMap(firstRow);

            // حداقل هدر ضروری
            if (!FindIndex(headerMap, "SubProjectCode").HasValue)
                throw new InvalidOperationException("Header 'SubProjectCode' is required.");

            foreach (var row in ws.RowsUsed().Skip(1))
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (row.IsEmpty()) continue;

                var code = GetCellString(row, headerMap, "SubProjectCode");
                if (string.IsNullOrWhiteSpace(code)) continue;

                var rec = new BudgetRecord(code.Trim())
                {
                    ContractTitle = GetCellString(row, headerMap, "ContractTitle"),
                    ContractNumber = GetCellString(row, headerMap, "ContractNumber"),

                    // تاریخ‌ها به‌صورت رشته yyyy-MM-dd
                    ContractDate = GetCellDateString(row, headerMap, "ContractDate"),
                    //AgentContractDate = GetCellDateString(row, headerMap, "AgentContractDate"),
                    StartDate = GetCellDateString(row, headerMap, "StartDate"),
                    EndDate = GetCellDateString(row, headerMap, "EndDate"),
                    ExtendedEndDate = GetCellDateString(row, headerMap, "ExtendedEndDate"),

                    Contractor = GetCellString(row, headerMap, "Contractor"),
                    Agent = GetCellString(row, headerMap, "Agent"),
                    //CompanyType = GetCellString(row, headerMap, "CompanyType"),
                    //AgentContractNumber = GetCellString(row, headerMap, "AgentContractNumber"),
                    //ExecutionType = GetCellString(row, headerMap, "ExecutionType"),
                    ContractStatus = GetCellString(row, headerMap, "ContractStatus"),

                    // اعداد و درصدها
                    TotalContractAmount = GetCellDecimal(row, headerMap, "TotalContractAmount"),
                    InitialAmount = GetCellDecimal(row, headerMap, "InitialAmount"),
                    //CurrentYearCashCredit = GetCellDecimal(row, headerMap, "CurrentYearCashCredit"),
                    //CurrentYearNonCashCredit = GetCellDecimal(row, headerMap, "CurrentYearNonCashCredit"),
                    CurrentYearTotalCredit = GetCellDecimal(row, headerMap, "CurrentYearTotalCredit"),
                    TotalCreditFromStart = GetCellDecimal(row, headerMap, "TotalCreditFromStart"),
                    TotalInvoicesAmount = GetCellDecimal(row, headerMap, "TotalInvoicesAmount"),
                    TotalWorkProgress = GetCellDecimal(row, headerMap, "TotalWorkProgress"),
                    CurrentYearInvoicesAmount = GetCellDecimal(row, headerMap, "CurrentYearInvoicesAmount"),

                    ExecutiveDept = GetCellString(row, headerMap, "ExecutiveDept"),
                    //ResponsibleDept = GetCellString(row, headerMap, "ResponsibleDept"),
                    //ActivityType = GetCellString(row, headerMap, "ActivityType"),
                    WorkReferralMethod = GetCellString(row, headerMap, "WorkReferralMethod"),
                    CreditNumber = GetCellString(row, headerMap, "CreditNumber"),
                    Nature = GetCellString(row, headerMap, "Nature"),
                };

                records.Add(rec);
            }

            return Task.FromResult<IEnumerable<BudgetRecord>>(records);
        }

        // نرمال‌سازی: حذف هرچیزی غیر از حروف/اعداد یونی‌کد و حروف کوچک
        private static string Normalize(string s) =>
            Regex.Replace(s ?? string.Empty, @"[^\p{L}\p{Nd}]", string.Empty)
                 .ToLowerInvariant();

        private static Dictionary<string, int> BuildHeaderMap(IXLRow headerRow)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var cell in headerRow.CellsUsed())
            {
                var raw = cell.GetString();
                if (string.IsNullOrWhiteSpace(raw)) continue;
                var key = Normalize(raw);
                if (!map.ContainsKey(key))
                    map[key] = cell.Address.ColumnNumber;
            }
            return map;
        }

        private static int? FindIndex(Dictionary<string, int> headerMap, string headerName)
        {
            // 1) خود نام
            var norm = Normalize(headerName);
            if (headerMap.TryGetValue(norm, out var idx)) return idx;

            // 2) معادل‌ها
            if (HeaderSynonyms.TryGetValue(headerName, out var syns))
            {
                foreach (var s in syns)
                {
                    var n = Normalize(s);
                    if (headerMap.TryGetValue(n, out var i)) return i;
                }
            }

            return null;
        }

        private static string? GetCellString(IXLRow row, Dictionary<string, int> headerMap, string header)
        {
            var idx = FindIndex(headerMap, header);
            if (!idx.HasValue) return null;

            var cell = row.Cell(idx.Value);
            var s = cell.GetString();
            return string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        }

        // تاریخ به رشته yyyy-MM-dd
        private static string? GetCellDateString(IXLRow row, Dictionary<string, int> headerMap, string header)
        {
            var idx = FindIndex(headerMap, header);
            if (!idx.HasValue) return null;

            var cell = row.Cell(idx.Value);
            // حالت DateTime واقعی
            if (cell.DataType == XLDataType.DateTime)
                return cell.GetDateTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            // حالت عدد + فرمت تاریخ
            var fmt = cell.Style.NumberFormat.Format ?? string.Empty;
            var looksLikeDate = fmt.IndexOf('y') >= 0 || fmt.IndexOf('d') >= 0 || fmt.IndexOf('m') >= 0;
            if (cell.DataType == XLDataType.Number && looksLikeDate)
            {
                try
                {
                    var dt = cell.GetDateTime();
                    return dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch { /* ignore */ }
            }

            // حالت متنی: تلاش برای parse و سپس فرمت
            var txt = cell.GetString();
            if (!string.IsNullOrWhiteSpace(txt))
            {
                if (DateTime.TryParse(txt, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt) ||
                    DateTime.TryParse(txt, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                // اگر نشد، همان متن تمیز برگشت داده شود
                return txt.Trim();
            }

            return null;
        }

        private static decimal? GetCellDecimal(IXLRow row, Dictionary<string, int> headerMap, string header)
        {
            var idx = FindIndex(headerMap, header);
            if (!idx.HasValue) return null;

            var cell = row.Cell(idx.Value);
            if (cell.IsEmpty()) return null;

            var fmt = cell.Style.NumberFormat.Format ?? string.Empty;
            var isPercent = fmt.Contains("%");

            // حالت عددی
            if (cell.DataType == XLDataType.Number)
            {
                try
                {
                    var val = Convert.ToDecimal(cell.GetDouble());
                    return isPercent ? val * 100m : val;
                }
                catch { /* ignore */ }
            }

            // حالت متنی
            var txt = cell.GetString()?.Trim();
            if (!string.IsNullOrEmpty(txt))
            {
                if (txt.EndsWith("%"))
                {
                    var num = txt[..^1].Trim();
                    if (decimal.TryParse(num, NumberStyles.Any, CultureInfo.InvariantCulture, out var p) ||
                        decimal.TryParse(num, NumberStyles.Any, CultureInfo.CurrentCulture, out p))
                        return p; // "45%" => 45
                }

                if (decimal.TryParse(txt, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ||
                    decimal.TryParse(txt, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                    return d;
            }

            return null;
        }
    }
}
