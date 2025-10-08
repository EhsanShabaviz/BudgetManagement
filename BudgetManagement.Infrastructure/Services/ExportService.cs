using BudgetManagement.Application.DTOs;
using BudgetManagement.Application.DTOs.Budget;
using BudgetManagement.Application.Interfaces;
using BudgetManagement.Common.Extensions;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.ComponentModel;
using System.Reflection;


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
                    page.Header().Text("گزارش لاگ کاربران سامانه محاسبه‌گر اعتبارات")
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


        public byte[] ExportBudgetReportToExcel(IEnumerable<BudgetReportDto> reports)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("BudgetReport");

            // تنظیم راست به چپ برای شیت
            ws.RightToLeft = true;

            // گرفتن لیست پراپرتی‌های DTO
            var properties = typeof(BudgetReportDto).GetProperties();

            // Header
            var headerRow = ws.Row(1);
            headerRow.Cell(1).Value = "ردیف";
            for (int i = 0; i < properties.Length; i++)
            {
                // اگر DisplayNameAttribute داشت، از اون استفاده کن
                var displayName = properties[i]
                    .GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                    ?? properties[i].Name;

                headerRow.Cell(i + 2).Value = displayName;
            }

            // استایل هدر: آبی، بولد، متن سفید، وسط‌چین
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#0d6efd"); // آبی شبیه اسکرین‌شات
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Font.FontColor = XLColor.White;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Rows
            int row = 2;
            int index = 1;
            foreach (var report in reports)
            {
                var currentRow = ws.Row(row);
                currentRow.Cell(1).Value = index++;
                for (int i = 0; i < properties.Length; i++)
                {
                    var value = properties[i].GetValue(report, null);
                    currentRow.Cell(i + 2).Value = value?.ToString() ?? "";
                }

                // رنگ alternating برای ردیف‌ها (سفید و خاکستری روشن)
                if (row % 2 == 0)
                    currentRow.Style.Fill.BackgroundColor = XLColor.White;
                else
                    currentRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#f8f9fa"); // خاکستری روشن

                // راست‌چین متن ردیف‌ها
                currentRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                row++;
            }

            // ردیف مجموع
            var totalRow = ws.Row(row);
            totalRow.Cell(1).Value = "مجموع";

            // محاسبه مجموع برای هر پراپرتی عددی
            for (int i = 0; i < properties.Length; i++)
            {
                var prop = properties[i];
                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType; // نوع زیرین nullable رو بگیر یا نوع اصلی

                if (propType == typeof(decimal) || propType == typeof(int) || propType == typeof(long) || propType == typeof(double))
                {
                    var sum = reports.Sum(r =>
                    {
                        var val = prop.GetValue(r);
                        return val != null ? Convert.ToDecimal(val) : 0m;
                    });
                    totalRow.Cell(i + 2).Value = sum;
                }
                else
                {
                    totalRow.Cell(i + 2).Value = ""; // برای پراپرتی‌های غیرعددی خالی بگذار
                }
            }

            // استایل ردیف مجموع: خاکستری، بولد، راست‌چین
            totalRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#e9ecef"); // خاکستری شبیه اسکرین‌شات
            totalRow.Style.Font.Bold = true;
            totalRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            // AutoFit برای زیبایی
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportBudgetReportToPdf(IEnumerable<BudgetReportDto> reports)
        {
            var properties = typeof(BudgetReportDto).GetProperties()
                .Where(prop => prop.Name != "ContractTitle"
                            && prop.Name != "Contractor"
                            && prop.Name != "Agent"
                            && prop.Name != "ContractStatus"
                            && prop.Name != "WorkReferralMethod") // حذف فیلد بر اساس نام پراپرتی
                .ToArray(); // تبدیل به آرایه برای استفاده آسان

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(10); // کاهش مارجین به 10
                    page.Size(PageSizes.A4.Landscape()); // برای جدول عریض
                    page.DefaultTextStyle(x => x.FontFamily("Vazir").FontSize(9)); // فونت سایز کوچکتر

                    // ✅ راست به چپ برای کل صفحه
                    page.ContentFromRightToLeft();

                    // ✅ هدر صفحه - وسط‌چین به صورت افقی
                    page.Header().AlignCenter().Height(40)
                        .Text("گزارش بودجه سامانه محاسبه‌گر اعتبارات")
                        .SemiBold().FontSize(14);

                    // ✅ فوتر با شماره صفحه - وسط‌چین به صورت افقی
                    page.Footer()
                        .AlignCenter()
                        .Height(20)
                        .Text(text =>
                        {
                            text.Span("صفحه ");
                            text.CurrentPageNumber();
                            text.Span(" از ");
                            text.TotalPages();
                        });

                    // ✅ محتوای جدول
                    page.Content().AlignCenter().Table(table =>
                    {
                        // تعریف ستون‌ها: یک ستون ثابت برای ردیف + بقیه پراپرتی‌ها
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); // ردیف، باریک‌تر
                            foreach (var _ in properties)
                                columns.RelativeColumn(1); // تقسیم مساوی با نسبت 1
                        });

                        // Header با استایل آبی - وسط‌چین
                        table.Header(header =>
                        {
                            header.Cell().Background("#0d6efd").Padding(5) // افزایش padding به 5
                                .Text("ردیف").Bold().FontColor(Colors.White).AlignCenter();
                            foreach (var prop in properties)
                            {
                                var displayName = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                                                  ?? prop.Name;
                                header.Cell().Background("#0d6efd").Padding(5)
                                    .Text(displayName).Bold().FontColor(Colors.White).AlignCenter();
                            }
                        });

                        // Rows با alternating color - وسط‌چین
                        int index = 1;
                        foreach (var report in reports)
                        {
                            string rowColor = (index % 2 == 0) ? Colors.White : "#f8f9fa"; // alternating سفید و خاکستری روشن

                            table.Cell().Background(rowColor).Padding(5) // افزایش padding
                                .Text(index++.ToString()).AlignCenter();
                            foreach (var prop in properties)
                            {
                                var value = prop.GetValue(report, null);

                                string text = value switch
                                {
                                    DateTime dt => dt.ToShamsiDateTime(),
                                    null => "",
                                    _ => value.ToString()
                                };

                                table.Cell().Background(rowColor).Padding(5)
                                    .Text(text).AlignCenter();
                            }
                        }

                        // ردیف مجموع با استایل خاکستری - وسط‌چین
                        table.Cell().Background("#e9ecef").Padding(5)
                            .Text("مجموع").Bold().AlignCenter();
                        foreach (var prop in properties)
                        {
                            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                            string totalText = "";
                            if (propType == typeof(decimal) || propType == typeof(int) || propType == typeof(long) || propType == typeof(double))
                            {
                                var sum = reports.Sum(r =>
                                {
                                    var val = prop.GetValue(r);
                                    return val != null ? Convert.ToDecimal(val) : 0m;
                                });
                                totalText = sum.ToString("N0");
                            }

                            table.Cell().Background("#e9ecef").Padding(5)
                                .Text(totalText).Bold().AlignCenter();
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
