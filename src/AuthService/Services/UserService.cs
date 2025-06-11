using System.Threading.Tasks;
using AuthService.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly AuthDbContext _db;

        public UserService(AuthDbContext db)
            => _db = db;

        public async Task<int?> ValidateCredentialsAsync(string username, string password)
        {
            // Ejecuta el SP y lee solo el primer resultado
            var loginResults = await _db.LoginResults
                .FromSqlInterpolated($@"
                    EXEC dbo.sp_ValidarUsuario 
                        @Username      = {username}, 
                        @PasswordInput = {password}
                ")
                .AsNoTracking()
                .ToListAsync();

            var login = loginResults.FirstOrDefault();
            return login?.IdUsuario;
        }
    }
}
