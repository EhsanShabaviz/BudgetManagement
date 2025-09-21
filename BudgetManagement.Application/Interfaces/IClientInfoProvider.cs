using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.Application.Interfaces;

public interface IClientInfoProvider
{
    string GetClientIpAddress();
    string GetBrowserInfo();
    // Or: string GetDeviceType(); string GetOS(); string GetLocation();
}
