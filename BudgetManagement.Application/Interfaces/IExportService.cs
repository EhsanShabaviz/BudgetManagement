using BudgetManagement.Application.DTOs;
using BudgetManagement.Application.DTOs.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.Application.Interfaces
{
    public interface IExportService
    {
        byte[] ExportAuditLogsToExcel(IEnumerable<AuditLogDto> logs);
        byte[] ExportAuditLogsToPdf(IEnumerable<AuditLogDto> logs);

        byte[] ExportBudgetReportToExcel(IEnumerable<BudgetReportDto> reports);
        byte[] ExportBudgetReportToPdf(IEnumerable<BudgetReportDto> reports);


    }
}
