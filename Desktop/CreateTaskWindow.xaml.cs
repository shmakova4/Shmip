using Desktop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Todo.Entities;

namespace Desktop
{
    /// <summary>
    /// Логика взаимодействия для CreateTaskWindow.xaml
    /// </summary>
    public partial class CreateTaskWindow : Window
    {
        public CreateTaskWindow()
        {
            InitializeComponent();
        }


        
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите название задачи",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
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
                    Username = CurrentUser.User.Username
                };

                var taskRepository = new TaskRepository();
                if (taskRepository.AddTask(task))
                {
                    Console.WriteLine($"Задача сохранена для пользователя: {CurrentUser.User.Username}");
                    this.DialogResult = true; 
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при сохранении задачи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Пользователь не авторизован", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
