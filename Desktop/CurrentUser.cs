using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Entities;

namespace Desktop
{
    class CurrentUser
    {
        public static UserModel? User { get; set; }

        public static string? Username => User?.Username;

        public static bool IsAuthenticated => User != null;
    }
}
