using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Todo.Entities;

namespace Desktop.Repository
{
    internal class UserRepository
    {
        private static List<UserModel> _users;
        private const string UsersFilePath = @"C:\Users\User\Desktop\2 курс\Todo\users.json";
        private static readonly object _lock = new object();

        public UserRepository()
        {
            LoadUsersFromFile();
        }

        public bool RegisterUser(string username, string email, string password)
        {
            lock (_lock)
            {
                if (_users.Any(u => u.Username == username || u.Email == email))
                {
                    return false;
                }

                (string hash, string salt) = HashPassword(password);

                UserModel newUser = new UserModel
                {
                    Username = username,
                    Email = email,
                    PasswordHash = hash,
                    PasswordSalt = salt
                };

                _users.Add(newUser);
                SaveUsersToFile();
                return true;
            }
        }

        public UserModel? AuthenticateUser(string email, string password)
        {
            lock (_lock)
            {
                UserModel? user = _users.FirstOrDefault(u => u.Email == email);
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
        }

        public UserModel? GetUserByEmail(string email)
        {
            lock (_lock)
            {
                return _users.FirstOrDefault(u => u.Email == email);
            }
        }

        public List<UserModel> GetAllUsers()
        {
            lock (_lock)
            {
                return new List<UserModel>(_users);
            }
        }

        private void LoadUsersFromFile()
        {
            lock (_lock)
            {
                try
                {
                    string fullPath = Path.GetFullPath(UsersFilePath);
                    Console.WriteLine($"Пытаемся загрузить пользователей из: {fullPath}");

                    if (File.Exists(UsersFilePath))
                    {
                        string json = File.ReadAllText(UsersFilePath);
                        Console.WriteLine($"Содержимое файла: {json}");

                        if (!string.IsNullOrEmpty(json))
                        {
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                                WriteIndented = true
                            };

                            var users = JsonSerializer.Deserialize<List<UserModel>>(json, options);
                            if (users != null)
                            {
                                _users = users;
                                Console.WriteLine($"Загружено {_users.Count} пользователей");
                            }
                            else
                            {
                                Console.WriteLine("Не удалось десериализовать JSON");
                                _users = new List<UserModel>();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Файл пустой");
                            _users = new List<UserModel>();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Файл users.json не найден");
                        _users = new List<UserModel>();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка загрузки пользователей: {ex.Message}");
                    Console.WriteLine($"StackTrace: {ex.StackTrace}");
                    _users = new List<UserModel>();
                }
            }
        }

        private void SaveUsersToFile()
        {
            lock (_lock)
            {
                try
                {
                    string fullPath = Path.GetFullPath(UsersFilePath);
                    Console.WriteLine($"Сохраняем пользователей в: {fullPath}");

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    string json = JsonSerializer.Serialize(_users, options);
                    File.WriteAllText(UsersFilePath, json, Encoding.UTF8);
                    Console.WriteLine($"Сохранено {_users.Count} пользователей в файл");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка сохранения пользователей: {ex.Message}");
                    Console.WriteLine($"StackTrace: {ex.StackTrace}");
                }
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