using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManagement.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    [Required, StringLength(256)]
    public string FullName { get; set; } = "";

    [Required, StringLength(10)]
    public string NationalCode { get; set; } = "";
}
