using BudgetManagement.Domain.Entities;
using System.Globalization;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace BudgetManagement.Infrastructure.ExternalServices
{
    /// <summary>
    /// Reads Excel stream and maps rows to BudgetRecord instances.
    /// Header matching supports Persian and English (unicode letters/digits).
    /// Dates are emitted as strings "yyyy-MM-dd" per project policy.
    /// </summary>
    public class ExcelUploaderService
    {
        // English -> synonyms (include Persian headers)
        private static readonly Dictionary<string, string[]> HeaderSynonyms = new(StringComparer.OrdinalIgnoreCase)
        {
            ["SubProjectCode"] = new[] { "کد زیرپروژه" },
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
            ["TotalInvoicesAmount"] = new[] { "مبلغ کل صورت وضعیت ها" },
            ["TotalWorkProgress"] = new[] { "مبلغ کل کارکرد" },
            ["CurrentYearInvoicesAmount"] = new[] { "مبلغ صورت وضعیت ها در سال جاری" },
            ["CreditNumber"] = new[] { "شماره تأمین", "شماره تامین" },
            ["Nature"] = new[] { "ماهیت" },
        };

        public async Task<List<BudgetRecord>> ReadBudgetRecordsAsync(Stream excelStream)
        {
            if (excelStream == null) throw new ArgumentNullException(nameof(excelStream));
            if (excelStream.CanSeek) excelStream.Position = 0;

            var records = new List<BudgetRecord>();
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);

            var firstRowUsed = worksheet.FirstRowUsed();
            var lastRowUsed = worksheet.LastRowUsed();
            if (firstRowUsed == null || lastRowUsed == null) return records;

            var headerRow = firstRowUsed.RowNumber();
            var lastRow = lastRowUsed.RowNumber();

            var headerMap = BuildHeaderMap(worksheet.Row(headerRow));

            for (int r = headerRow + 1; r <= lastRow; r++)
            {
                var row = worksheet.Row(r);
                if (row.IsEmpty()) continue;

                var subProjectCode = GetCellString(row, headerMap, "SubProjectCode");
                if (string.IsNullOrWhiteSpace(subProjectCode)) continue;

                var record = new BudgetRecord(subProjectCode.Trim())
                {
                    ContractTitle = GetCellString(row, headerMap, "ContractTitle"),
                    ContractNumber = GetCellString(row, headerMap, "ContractNumber"),
                    ContractDate = GetCellString(row, headerMap, "ContractDate"),
                    Contractor = GetCellString(row, headerMap, "Contractor"),
                    Agent = GetCellString(row, headerMap, "Agent"),
                    //CompanyType = GetCellString(row, headerMap, "CompanyType"),
                    //AgentContractNumber = GetCellString(row, headerMap, "AgentContractNumber"),
                    //AgentContractDate = GetCellString(row, headerMap, "AgentContractDate"),
                    //ExecutionType = GetCellString(row, headerMap, "ExecutionType"),
                    ContractStatus = GetCellString(row, headerMap, "ContractStatus"),
                    TotalContractAmount = GetCellDecimal(row, headerMap, "TotalContractAmount"),
                    InitialAmount = GetCellDecimal(row, headerMap, "InitialAmount"),
                    //CurrentYearCashCredit = GetCellDecimal(row, headerMap, "CurrentYearCashCredit"),
                    //CurrentYearNonCashCredit = GetCellDecimal(row, headerMap, "CurrentYearNonCashCredit"),
                    CurrentYearTotalCredit = GetCellDecimal(row, headerMap, "CurrentYearTotalCredit"),
                    TotalCreditFromStart = GetCellDecimal(row, headerMap, "TotalCreditFromStart"),
                    ExecutiveDept = GetCellString(row, headerMap, "ExecutiveDept"),
                    //ResponsibleDept = GetCellString(row, headerMap, "ResponsibleDept"),
                    StartDate = GetCellString(row, headerMap, "StartDate"),
                    EndDate = GetCellString(row, headerMap, "EndDate"),
                    ExtendedEndDate = GetCellString(row, headerMap, "ExtendedEndDate"),
                    //ActivityType = GetCellString(row, headerMap, "ActivityType"),
                    WorkReferralMethod = GetCellString(row, headerMap, "WorkReferralMethod"),
                    TotalInvoicesAmount = GetCellDecimal(row, headerMap, "TotalInvoicesAmount"),
                    TotalWorkProgress = GetCellDecimal(row, headerMap, "TotalWorkProgress"),
                    CurrentYearInvoicesAmount = GetCellDecimal(row, headerMap, "CurrentYearInvoicesAmount"),
                    CreditNumber = GetCellString(row, headerMap, "CreditNumber"),
                    Nature = GetCellString(row, headerMap, "Nature"),
                };

                records.Add(record);
            }

            return await Task.FromResult(records);
        }

        private static Dictionary<string, int> BuildHeaderMap(IXLRow headerRow)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var lastCell = headerRow.LastCellUsed();
            if (lastCell == null) return map;

            int lastCol = lastCell.Address.ColumnNumber;
            for (int c = 1; c <= lastCol; c++)
            {
                var raw = headerRow.Cell(c).GetString();
                if (string.IsNullOrWhiteSpace(raw)) continue;

                var key = NormalizeHeader(raw);
                if (!map.ContainsKey(key)) map[key] = c;
            }
            return map;
        }

        private static string NormalizeHeader(string s)
            => Regex.Replace(s ?? string.Empty, @"[^\p{L}\p{Nd}]", string.Empty)
                    .ToLowerInvariant();

        private static int? FindIndex(Dictionary<string, int> headerMap, string targetHeader)
        {
            // 1) مستقیم با نام هدف
            var norm = NormalizeHeader(targetHeader);
            if (headerMap.TryGetValue(norm, out var idx)) return idx;

            // 2) با معادل‌های هدف (مثلاً فارسی)
            if (HeaderSynonyms.TryGetValue(targetHeader, out var synonyms))
            {
                foreach (var syn in synonyms)
                {
                    var n = NormalizeHeader(syn);
                    if (headerMap.TryGetValue(n, out var i)) return i;
                }
            }

            return null;
        }

        private static string? GetCellString(IXLRow row, Dictionary<string, int> headerMap, string headerName)
        {
            var idx = FindIndex(headerMap, headerName);
            if (idx == null) return null;

            var cell = row.Cell(idx.Value);

            // متن موجود
            var val = cell.GetString();
            if (!string.IsNullOrWhiteSpace(val))
                return val.Trim();

            // تاریخ واقعی
            if (cell.DataType == XLDataType.DateTime)
                return cell.GetDateTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            // تاریخ عددی با فرمت تاریخ
            var fmt = cell.Style.NumberFormat.Format ?? string.Empty;
            var looksLikeDate = fmt.Contains("y", StringComparison.OrdinalIgnoreCase)
                                || fmt.Contains("d", StringComparison.OrdinalIgnoreCase)
                                || fmt.Contains("m", StringComparison.OrdinalIgnoreCase);
            if (cell.DataType == XLDataType.Number && looksLikeDate)
            {
                try
                {
                    var dt = cell.GetDateTime();
                    return dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                catch { /* ignore */ }
            }

            return null;
        }

        private static decimal? GetCellDecimal(IXLRow row, Dictionary<string, int> headerMap, string headerName)
        {
            var idx = FindIndex(headerMap, headerName);
            if (idx == null) return null;

            var cell = row.Cell(idx.Value);
            if (cell.IsEmpty()) return null;

            var fmt = cell.Style.NumberFormat.Format ?? string.Empty;
            var isPercent = fmt.Contains("%");

            if (cell.DataType == XLDataType.Number)
            {
                try
                {
                    var val = Convert.ToDecimal(cell.GetDouble());
                    return isPercent ? val * 100m : val;
                }
                catch { /* ignore */ }
            }

            var txt = cell.GetString()?.Trim();
            if (!string.IsNullOrEmpty(txt))
            {
                if (txt.EndsWith("%"))
                {
                    var num = txt[..^1].Trim();
                    if (decimal.TryParse(num, NumberStyles.Any, CultureInfo.InvariantCulture, out var p) ||
                        decimal.TryParse(num, NumberStyles.Any, CultureInfo.CurrentCulture, out p))
                        return p;
                }

                if (decimal.TryParse(txt, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ||
                    decimal.TryParse(txt, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                    return d;
            }

            return null;
        }
    }
}
