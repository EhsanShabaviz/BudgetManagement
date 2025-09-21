using BudgetManagement.Application.Interfaces;
using BudgetManagement.Domain.Entities;
using BudgetManagement.Infrastructure.Persistence;

namespace BudgetManagement.Infrastructure.Services;

public class AuditLogger : IAuditLogger
{
    private readonly BudgetManagementDbContext _context;
    private readonly IClientInfoProvider _clientInfo;

    public AuditLogger(BudgetManagementDbContext context, IClientInfoProvider clientInfo)
    {
        _context = context;
        _clientInfo = clientInfo;
    }

    public async Task LogAsync(string userId, string userName, string actionType, string? customDescription = null)
    {
        userName ??= "Unknown";

        var description = !string.IsNullOrWhiteSpace(customDescription)
            ? $"{userName}: {actionType} \" {customDescription} \""
            : $"{userName}: {actionType}";

        var log = new AuditLog
        {
            UserId = userId,
            ActionType = actionType,
            Description = description,
            DateTime = DateTime.Now,
            IPAddress = _clientInfo.GetClientIpAddress(),
            BrowserInfo = _clientInfo.GetBrowserInfo()
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
