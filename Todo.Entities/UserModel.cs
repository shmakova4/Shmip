using System.Text.Json.Serialization;

namespace Todo.Entities
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Email { get; set; } 
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public UserModel()
        {
            Username = string.Empty;
            Email = string.Empty; 
            PasswordHash = string.Empty;
            PasswordSalt = string.Empty;
        }
    }
}