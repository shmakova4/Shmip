using Desktop.Repository;
using Desktop.View;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
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
        private bool _isNavigating = false;

        public MainWindow()
        {
            InitializeComponent();
            _userRepository = new UserRepository();
            string filePath = @"C:\Users\User\Desktop\2 курс\Todo\users.json";
            Console.WriteLine($"Путь к файлу пользователей: {filePath}");
            Console.WriteLine($"Файл существует: {File.Exists(filePath)}");

            MainFrame.Navigated += MainFrame_Navigated;
            this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            StartLogoAnimation();
        }

        private void StartLogoAnimation()
        {
            var pulseAnimation = (Storyboard)FindResource("LogoPulseAnimation");
            if (pulseAnimation != null && LogoImage != null)
            {
                Storyboard.SetTarget(pulseAnimation, LogoImage);
                pulseAnimation.Begin();
            }
        }

        private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainFrame.Content is Page currentPage)
            {
                if (currentPage is HistoryPage || currentPage is CreateTaskPage)
                {
                    e.Cancel = true;

                    bool userHasTasks = CheckIfUserHasTasks(CurrentUser.User?.Username);

                    if (userHasTasks)
                    {
                        await NavigateToPageAsync(new MainPage());
                    }
                    else
                    {
                        await NavigateToPageAsync(new MainEmptyPage());
                    }
                }
            }
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            UpdateWindowTitle();
        }

        private void UpdateWindowTitle()
        {
            if (MainFrame.Content is Page currentPage)
            {
                if (currentPage is RegistrationPage)
                {
                    this.Title = "Регистрация";
                }
                else if (currentPage is MainEmptyPage)
                {
                    this.Title = "Добро пожаловать!";
                }
                else if (currentPage is CreateTaskPage)
                {
                    this.Title = "Создание задачи";
                }
                else if (currentPage is MainPage)
                {
                    this.Title = "Задачи";
                }
                else if (currentPage is HistoryPage)
                {
                    this.Title = "История выполненных задач";
                }
                else
                {
                    this.Title = "Todo App";
                }
            }
            else
            {
                this.Title = "Log In";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPageAsync(new RegistrationPage());
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
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
                    await NavigateToPageAsync(new MainPage());
                }
                else
                {
                    await NavigateToPageAsync(new MainEmptyPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при авторизации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Исключение: {ex}");
            }
        }

        public async Task NavigateToPageAsync(Page page)
        {
            if (_isNavigating) return;
            _isNavigating = true;

            try
            {
                if (MainFrame.Visibility != Visibility.Visible)
                {
                    MainFrame.Visibility = Visibility.Visible;
                    LoginGrid.Visibility = Visibility.Collapsed;
                    MainGrid.RowDefinitions[0].Height = new GridLength(0);
                    MainGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                }

                var exitAnimation = (Storyboard)FindResource("PageExitAnimation");
                if (MainFrame.Content != null && exitAnimation != null)
                {
                    Storyboard.SetTarget(exitAnimation, MainFrame);
                    exitAnimation.Begin();
                    await Task.Delay(200);
                }

                MainFrame.Navigate(page);

                var enterAnimation = (Storyboard)FindResource("PageEnterAnimation");
                if (enterAnimation != null)
                {
                    Storyboard.SetTarget(enterAnimation, MainFrame);
                    enterAnimation.Begin();
                }
            }
            finally
            {
                _isNavigating = false;
            }
        }

        public async void NavigateBackToLogin()
        {
            if (_isNavigating) return;
            _isNavigating = true;

            try
            {
                var exitAnimation = (Storyboard)FindResource("PageExitAnimation");
                if (MainFrame.Content != null && exitAnimation != null)
                {
                    Storyboard.SetTarget(exitAnimation, MainFrame);
                    exitAnimation.Begin();
                    await Task.Delay(200);
                }

                MainFrame.Visibility = Visibility.Collapsed;
                LoginGrid.Visibility = Visibility.Visible;
                MainGrid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                MainGrid.RowDefinitions[1].Height = new GridLength(0);

                MainFrame.Content = null;
                this.Title = "Log In";
            }
            finally
            {
                _isNavigating = false;
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
                if (string.IsNullOrEmpty(username))
                {
                    return false;
                }
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