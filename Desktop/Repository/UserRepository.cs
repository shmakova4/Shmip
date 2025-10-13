using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Todo.Entities;

namespace Desktop.Repository
{

    internal class UserRepository
    {
        private static List<UserModel> _users = new List<UserModel>();

        public bool RegisterUser(string username, string password)
        {
            if (_users.Any(u => u.Username == username))
            {
                return false; 
            }

            (string hash, string salt) = HashPassword(password);

            UserModel newUser = new UserModel
            {
                Username = username,
                PasswordHash = hash,
                PasswordSalt = salt
            };

            _users.Add(newUser);
            return true;
        }

        public UserModel? AuthenticateUser(string username, string password)
        {
            UserModel? user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null) return null;

            if (VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        private static (string Hash, string Salt) HashPassword(string password)
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            string salt = Convert.ToBase64String(saltBytes);

            string passwordWithSalt = password + salt;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordWithSalt);
            byte[] hashBytes = SHA256.Create().ComputeHash(passwordBytes);
            string hash = Convert.ToBase64String(hashBytes);

            return (hash, salt);
        }

        private static bool VerifyPassword(string password, string hash, string salt)
        {
            string passwordWithSalt = password + salt;
            byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordWithSalt);
            byte[] hashBytes = SHA256.Create().ComputeHash(passwordBytes);
            string hashedPassword = Convert.ToBase64String(hashBytes);

            return hashedPassword == hash;
        }
    }
}
