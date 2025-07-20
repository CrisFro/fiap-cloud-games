using Fcg.Domain.Services;
using Microsoft.AspNetCore.Identity;

namespace Fcg.Application.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string Hash(string senha) =>
            _hasher.HashPassword(null, senha);

        public bool Verify(string hash, string senha) =>
            _hasher.VerifyHashedPassword(null, hash, senha) == PasswordVerificationResult.Success;
    }
}
