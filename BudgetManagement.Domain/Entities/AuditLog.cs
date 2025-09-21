using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string ActionType { get; set; } = string.Empty; // Login, CreateUser, DeleteRole, ...

    public string Description { get; set; } = string.Empty;

    public DateTime DateTime { get; set; }

    public string IPAddress { get; set; } = string.Empty;

    public string BrowserInfo { get; set; } = string.Empty;
}

