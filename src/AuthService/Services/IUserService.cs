using System.Threading.Tasks;

namespace AuthService.Services
{
    public interface IUserService
    {
        Task<int?> ValidateCredentialsAsync(string username, string password);
    }
}
