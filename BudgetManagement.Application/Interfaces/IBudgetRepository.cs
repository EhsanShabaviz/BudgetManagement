using BudgetManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.Application.Interfaces;

public interface IBudgetRepository
{
    Task<IEnumerable<BudgetRecord>> GetAllAsync();
    Task<BudgetRecord?> GetBySubProjectCodeAsync(string subProjectCode);
    Task AddAsync(BudgetRecord record);
    Task UpdateAsync(BudgetRecord record);
    Task DeleteAsync(string subProjectCode);
}