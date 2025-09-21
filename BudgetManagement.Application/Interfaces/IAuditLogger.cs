using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.Application.Interfaces;

public interface IAuditLogger
{
    Task LogAsync(string userId, string userName, string actionType, string? customDescription = null);
}
