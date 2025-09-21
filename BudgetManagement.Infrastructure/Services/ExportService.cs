using BudgetManagement.Application.DTOs;
using BudgetManagement.Application.Interfaces;
using BudgetManagement.Common.Extensions;
using ClosedXML.Excel;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BudgetManagement.Infrastructure.Services
{
    public class ExportService : IExportService
    {
        public byte[] ExportAuditLogsToExcel(IEnumerable<AuditLogDto> logs)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("AuditLogs");

            // Header
            ws.Cell(1, 1).Value = "ردیف";
            ws.Cell(1, 2).Value = "کاربر";
            ws.Cell(1, 3).Value = "عملیات";
            ws.Cell(1, 4).Value = "توضیحات";
            ws.Cell(1, 5).Value = "تاریخ";
            ws.Cell(1, 6).Value = "IP";
            ws.Cell(1, 7).Value = "مرورگر";

            int row = 2;
            int index = 1;
            foreach (var log in logs)
            {
                ws.Cell(row, 1).Value = index++;
                ws.Cell(row, 2).Value = log.UserName ?? "";
                ws.Cell(row, 3).Value = log.ActionType ?? "";
                ws.Cell(row, 4).Value = log.Description ?? "";
                ws.Cell(row, 5).Value = log.DateTime.ToShamsiDateTime();
                ws.Cell(row, 6).Value = log.IPAddress ?? "";
                ws.Cell(row, 7).Value = log.BrowserInfo ?? "";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportAuditLogsToPdf(IEnumerable<AuditLogDto> logs)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    // ✅ هدر
                    page.Header().Text("گزارش لاگ کاربران سامانه نظام بودجه و اعتبارات")
                        .FontFamily("Vazir")
                        .SemiBold().FontSize(16).AlignCenter();

                    // ✅ فوتر با شماره صفحه
                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("صفحه ").FontFamily("Vazir");
                            text.CurrentPageNumber().FontFamily("Vazir");
                            text.Span(" از ").FontFamily("Vazir");
                            text.TotalPages().FontFamily("Vazir");
                        });

                    // ✅ محتوای جدول
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); // ردیف
                            columns.RelativeColumn();   // کاربر
                            columns.RelativeColumn();   // عملیات
                            columns.RelativeColumn(2);  // توضیحات
                            columns.RelativeColumn();   // تاریخ
                            columns.RelativeColumn();   // IP
                            columns.RelativeColumn();   // مرورگر
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Text("ردیف").FontFamily("Vazir").Bold();
                            header.Cell().Text("کاربر").FontFamily("Vazir").Bold();
                            header.Cell().Text("عملیات").FontFamily("Vazir").Bold();
                            header.Cell().Text("توضیحات").FontFamily("Vazir").Bold();
                            header.Cell().Text("تاریخ").FontFamily("Vazir").Bold();
                            header.Cell().Text("IP").FontFamily("Vazir").Bold();
                            header.Cell().Text("مرورگر").FontFamily("Vazir").Bold();
                        });

                        int index = 1;
                        foreach (var log in logs)
                        {
                            table.Cell().Text(index++.ToString()).FontFamily("Vazir");
                            table.Cell().Text(log.UserName ?? "").FontFamily("Vazir");
                            table.Cell().Text(log.ActionType ?? "").FontFamily("Vazir");
                            table.Cell().Text(log.Description ?? "").FontFamily("Vazir");

                            // ✅ تاریخ شمسی
                            table.Cell().Text(log.DateTime.ToShamsiDateTime()).FontFamily("Vazir");

                            table.Cell().Text(log.IPAddress ?? "").FontFamily("Vazir");
                            table.Cell().Text(log.BrowserInfo ?? "").FontFamily("Vazir");
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }


    }
}
