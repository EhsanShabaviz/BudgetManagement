using BudgetManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace BudgetManagement.Infrastructure.ExternalServices
{
    /// <summary>
    /// Reads Excel stream and maps rows to BudgetRecord instances.
    /// Header matching is done case-insensitive and ignores non-alphanumeric chars.
    /// Date fields are stored as strings (per project policy).
    /// </summary>
    public class ExcelUploaderService
    {
        public async Task<List<BudgetRecord>> ReadBudgetRecordsAsync(Stream excelStream)
        {
            if (excelStream == null) throw new ArgumentNullException(nameof(excelStream));

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
                var subProjectCode = GetCellString(row, headerMap, "SubProjectCode");
                if (string.IsNullOrWhiteSpace(subProjectCode))
                    continue;

                var record = new BudgetRecord(subProjectCode.Trim())
                {
                    ContractTitle = GetCellString(row, headerMap, "ContractTitle"),
                    ContractNumber = GetCellString(row, headerMap, "ContractNumber"),
                    ContractDate = GetCellString(row, headerMap, "ContractDate"),
                    Contractor = GetCellString(row, headerMap, "Contractor"),
                    Agent = GetCellString(row, headerMap, "Agent"),
                    CompanyType = GetCellString(row, headerMap, "CompanyType"),
                    AgentContractNumber = GetCellString(row, headerMap, "AgentContractNumber"),
                    AgentContractDate = GetCellString(row, headerMap, "AgentContractDate"),
                    ExecutionType = GetCellString(row, headerMap, "ExecutionType"),
                    ContractStatus = GetCellString(row, headerMap, "ContractStatus"),
                    TotalContractAmount = GetCellDecimal(row, headerMap, "TotalContractAmount"),
                    InitialAmount = GetCellDecimal(row, headerMap, "InitialAmount"),
                    CurrentYearCashCredit = GetCellDecimal(row, headerMap, "CurrentYearCashCredit"),
                    CurrentYearNonCashCredit = GetCellDecimal(row, headerMap, "CurrentYearNonCashCredit"),
                    CurrentYearTotalCredit = GetCellDecimal(row, headerMap, "CurrentYearTotalCredit"),
                    TotalCreditFromStart = GetCellDecimal(row, headerMap, "TotalCreditFromStart"),
                    ExecutiveDept = GetCellString(row, headerMap, "ExecutiveDept"),
                    ResponsibleDept = GetCellString(row, headerMap, "ResponsibleDept"),
                    StartDate = GetCellString(row, headerMap, "StartDate"),
                    EndDate = GetCellString(row, headerMap, "EndDate"),
                    ExtendedEndDate = GetCellString(row, headerMap, "ExtendedEndDate"),
                    ActivityType = GetCellString(row, headerMap, "ActivityType"),
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
            => Regex.Replace(s ?? string.Empty, "[^a-zA-Z0-9]", string.Empty).ToLowerInvariant();

        private static string? GetCellString(IXLRow row, Dictionary<string, int> headerMap, string headerName)
        {
            var idx = FindIndex(headerMap, headerName);
            if (idx == null) return null;

            var val = row.Cell(idx.Value).GetString();
            if (string.IsNullOrWhiteSpace(val))
            {
                // If it's a date type in Excel, format to string
                var cell = row.Cell(idx.Value);
                if (cell.DataType == XLDataType.DateTime)
                    return cell.GetDateTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                return null;
            }
            return val.Trim();
        }

        private static decimal? GetCellDecimal(IXLRow row, Dictionary<string, int> headerMap, string headerName)
        {
            var idx = FindIndex(headerMap, headerName);
            if (idx == null) return null;
            var cell = row.Cell(idx.Value);
            if (cell.IsEmpty()) return null;

            if (cell.DataType == XLDataType.Number)
            {
                try { return Convert.ToDecimal(cell.GetDouble()); }
                catch { /* ignore */ }
            }

            var txt = cell.GetString();
            if (decimal.TryParse(txt, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return d;
            if (decimal.TryParse(txt, NumberStyles.Any, CultureInfo.CurrentCulture, out d)) return d;
            return null;
        }

        private static int? FindIndex(Dictionary<string, int> headerMap, string targetHeader)
        {
            var norm = NormalizeHeader(targetHeader);
            if (headerMap.TryGetValue(norm, out var idx)) return idx;
            foreach (var kv in headerMap)
            {
                if (kv.Key.Equals(norm, StringComparison.OrdinalIgnoreCase)) return kv.Value;
            }
            return null;
        }
    }
}
