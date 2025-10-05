using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Desktop
{
    public class InputValidator
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false; // Или можно бросить исключение, если пустой email не допускается
            }

            // Простой regex для проверки "@" и "."
            // Лучше использовать более строгий regex для реальной проверки email в production.
            // Например, можно взять regex из System.ComponentModel.DataAnnotations.EmailAddressAttribute.
            // Но для простоты и соответствия требованиям задачи, вот этот пример.
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false; // Или можно бросить исключение, если пустой пароль не допускается
            }
            return password.Length >= 6;
        }
    }

}
