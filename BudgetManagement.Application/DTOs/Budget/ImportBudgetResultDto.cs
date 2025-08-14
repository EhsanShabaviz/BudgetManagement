using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.Application.DTOs.Budget
{
    public class ImportBudgetResultDto
    {
        public int InsertedCount { get; set; }
        public int UpdatedCount { get; set; }
        public List<string> Errors { get; set; } = new();

        public ImportBudgetResultDto()
        {
        }

        public ImportBudgetResultDto(int insertedCount, int updatedCount, List<string>? errors = null)
        {
            InsertedCount = insertedCount;
            UpdatedCount = updatedCount;
            Errors = errors ?? new List<string>();
        }
    }
}

