using BudgetManagement.Application.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BudgetManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(string usernameOrEmail, string password, bool rememberMe);
        Task LogoutAsync(string userId);
    }
}
