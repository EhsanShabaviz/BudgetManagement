using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BudgetManagement.Infrastructure.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; } = "";
    }
}
