using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Entities
{
    public class UserModel
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public UserModel()
        {
            Username = string.Empty;
            PasswordHash = string.Empty;
            PasswordSalt = string.Empty;
        }
    }

}
