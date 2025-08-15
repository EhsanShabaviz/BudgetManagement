using System.Collections.Generic;
using BudgetManagement.Domain.Entities;

namespace BudgetManagement.Application.DTOs.Budget
{
    public class BudgetValidationResultDto
    {
        public List<string> Errors { get; } = new();
        public bool HasErrors => Errors.Count > 0;
        public List<BudgetRecord> ValidRecords { get; set; } = new();
    }
}
