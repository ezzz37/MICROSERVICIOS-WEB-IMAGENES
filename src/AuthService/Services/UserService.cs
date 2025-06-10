using AuthService.Data;
using AuthService.Helpers;
using AuthService.Models;
using Microsoft.Extensions.Options;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly AuthDbContext _db;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public UserService(
            AuthDbContext db,
            IOptions<EncryptionOptions> encOpts)
        {
            _db = db;
            _key = Convert.FromBase64String(encOpts.Value.Key);
            _iv = Convert.FromBase64String(encOpts.Value.IV);
        }

        public Usuario? ValidateCredentials(string username, string password)
        {
            var user = _db.Usuarios
                          .SingleOrDefault(u => u.Username == username);
            if (user == null) return null;

            var plain = EncryptionHelper.Decrypt(user.PasswordEnc, _key, _iv);
            return plain == password ? user : null;
        }
    }
}
