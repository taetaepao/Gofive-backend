using Microsoft.AspNetCore.Identity;
using Organize.Models.Domain;

namespace Organize.Controllers
{
    public class PasswordHasher
    {

        private readonly PasswordHasher<Users> _passwordHasher = new();

        public string HashPassword(Users user, string plainPassword)
        {
            return _passwordHasher.HashPassword(user, plainPassword);
        }

        public bool VerifyPassword(Users user, string hashedPassword, string plainPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
