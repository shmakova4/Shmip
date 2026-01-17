using Desktop.Repository;
using System;
using System.Windows;
using System.Windows.Controls;
using Todo.Entities;

namespace Desktop.View
{
    public partial class CreateTaskPage : Page
    {
        private TaskRepository _taskRepository = new TaskRepository();

        public CreateTaskPage()
        {
            InitializeComponent();
            DatePickerControl.SelectedDate = DateTime.Today;
            HoursComboBox.SelectedIndex = 0;
            MinutesComboBox.SelectedIndex = 0;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите название задачи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                TitleTextBox.Focus();
                return;
            }

            string title = TitleTextBox.Text;
            string category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Дом";
            string description = DescriptionTextBox.Text;

            DateTime? selectedDate = DatePickerControl.SelectedDate;
            string hourStr = (HoursComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "09";
            string minuteStr = (MinutesComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "00";

            DateTime taskDate;
            if (selectedDate.HasValue)
            {
                taskDate = selectedDate.Value;
            }
            else
            {
                taskDate = DateTime.Today;
            }

            string time = $"{hourStr}:{minuteStr}";

            if (CurrentUser.IsAuthenticated && CurrentUser.User != null)
            {
                var task = new TaskModel
                {
                    Title = title,
                    Category = category,
                    Description = description,
                    Date = taskDate,
                    Time = time,
                    Username = CurrentUser.User.Username,
                    IsCompleted = false,
                    CreatedAt = DateTime.Now
                };

                if (_taskRepository.AddTask(task))
                {
                    bool userHasTasks = _taskRepository.UserHasTasks(CurrentUser.User.Username);

                    if (userHasTasks)
                    {
                        NavigationService.Navigate(new MainPage());
                    }
                    else
                    {
                        NavigationService.Navigate(new MainEmptyPage());
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка при сохранении задачи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пользователь не авторизован", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null)
            {
                bool userHasTasks = _taskRepository.UserHasTasks(CurrentUser.User?.Username);

                if (userHasTasks)
                {
                    NavigationService.Navigate(new MainPage());
                }
                else
                {
                    NavigationService.Navigate(new MainEmptyPage());
                }
            }
        }
    }
}