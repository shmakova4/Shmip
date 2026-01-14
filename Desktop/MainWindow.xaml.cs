using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Desktop.Repository;
using Todo.Entities;

namespace Desktop
{
    public class EmptyStringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class MainWindow : Window
    {
        private readonly UserRepository _userRepository;

        public MainWindow()
        {
            InitializeComponent();
            _userRepository = new UserRepository();
            string filePath = @"C:\Users\User\Desktop\2 курс\Todo\users.json";
            Console.WriteLine($"Путь к файлу пользователей: {filePath}");
            Console.WriteLine($"Файл существует: {File.Exists(filePath)}");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Registration registrationWindow = new Registration();
            registrationWindow.Show();
            this.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Text;

            if (!InputValidator.IsValidEmail(email))
            {
                MessageBox.Show("Пожалуйста, введите корректный email.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!InputValidator.IsValidPassword(password))
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var allUsers = _userRepository.GetAllUsers();
                Console.WriteLine($"Всего пользователей в системе: {allUsers.Count}");

                foreach (var userItem in allUsers)
                {
                    Console.WriteLine($"Пользователь: {userItem.Username}, Email: {userItem.Email}");
                }

                var authenticatedUser = _userRepository.AuthenticateUser(email, password);

                if (authenticatedUser == null)
                {
                    MessageBox.Show("Неверный email или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    CreateTestUserIfNeeded(email, password, _userRepository);
                    return;
                }

                CurrentUser.User = authenticatedUser;
                bool userHasTasks = CheckIfUserHasTasks(authenticatedUser.Username);

                if (userHasTasks)
                {
                    Main mainWindow = new Main();
                    mainWindow.Show();
                }
                else
                {
                    Main_empty mainEmptyWindow = new Main_empty();
                    mainEmptyWindow.Show();
                }

                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Исключение: {ex}");
            }
        }

        private void CreateTestUserIfNeeded(string email, string password, UserRepository userRepository)
        {
            try
            {
                string username = email.Contains('@') ? email.Split('@')[0] : email;
                if (userRepository.RegisterUser(username, email, password))
                {
                    MessageBox.Show($"Создан новый пользователь: {email}\nПопробуйте войти снова.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    Console.WriteLine($"Создан тестовый пользователь: {username} ({email})");
                }
                else
                {
                    Console.WriteLine($"Не удалось создать пользователя {email} - возможно уже существует");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка создания тестового пользователя: {ex.Message}");
            }
        }

        private bool CheckIfUserHasTasks(string username)
        {
            try
            {
                var taskRepository = new TaskRepository();
                return taskRepository.UserHasTasks(username);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка проверки задач: {ex.Message}");
                return false;
            }
        }
    }
}