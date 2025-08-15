using System.Collections.Generic;
using BudgetManagement.Domain.Entities;

namespace BudgetManagement.Application.DTOs.Budget
{
    public class ImportBudgetResultDto
    {
        // آمار
        public int TotalParsed { get; set; }
        public int TotalValid { get; set; }
        public int InsertedCount { get; set; }
        public int UpdatedCount { get; set; }

        // خطاها
        public List<string> Errors { get; set; } = new();

        // رکوردهای معتبر برای نمایش در Preview
        public List<BudgetRecord> ValidRecords { get; set; } = new();
    }
}
