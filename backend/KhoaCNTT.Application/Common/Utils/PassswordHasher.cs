
using BCrypt.Net;

namespace KhoaCNTT.Application.Common.Utils
{
    public class PasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        } // hash kèm salt

        public bool Verify(string passwordHash, string inputPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, passwordHash);
        }
    }
}