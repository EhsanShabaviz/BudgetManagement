using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.Infrastructure.Identity
{
    public class ApplicationRole : IdentityRole
    {
        [Required(ErrorMessage = "نام نقش الزامی است")]
        public override string? Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "توضیحات نقش الزامی است")]
        public string Description { get; set; } = string.Empty;
    }
}
