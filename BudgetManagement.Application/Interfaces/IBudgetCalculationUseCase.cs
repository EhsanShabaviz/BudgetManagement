using BudgetManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.Application.Interfaces
{
    public interface IBudgetCalculationUseCase
    {
        Task CalculateAsync(List<BudgetRecord> records);
    }

}
