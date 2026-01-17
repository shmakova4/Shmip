using Desktop.Repository;
using Desktop.View;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Todo.Entities;

namespace Desktop.View
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            if (string.IsNullOrEmpty(text))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class RegistrationPage : Page
    {
        private UserRepository _userRepository = new UserRepository();

        public RegistrationPage()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.NavigateBackToLogin();
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Text;
            string confirmPassword = ConfirmPasswordTextBox.Text;

            if (!InputValidator.IsValidUsername(username))
            {
                MessageBox.Show("Имя пользователя должно содержать не менее трех знаков.");
                return;
            }

            if (!InputValidator.IsValidEmail(email))
            {
                MessageBox.Show("Введите корректный адрес электронной почты.");
                return;
            }

            if (!InputValidator.IsValidPassword(password))
            {
                MessageBox.Show("Пароль должен состоять не менее чем из шести символов.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            if (_userRepository.RegisterUser(username, email, password))
            {
                UserModel registeredUser = _userRepository.GetUserByEmail(email);

                if (registeredUser != null)
                {
                    CurrentUser.User = registeredUser;
                    MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        await mainWindow.NavigateToPageAsync(new MainEmptyPage());
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка при получении данных пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Имя пользователя или email уже заняты. Пожалуйста, выберите другие.");
            }
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}