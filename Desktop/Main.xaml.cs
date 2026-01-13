using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Desktop
{
    public partial class Main : Window
    {
        private Border _selectedTaskItem;
        private bool _isTask1Completed = false;
        private bool _isTask2Completed = true;
        private bool _isTask3Completed = false;
        private bool _isTask4Completed = false;
        private bool _isTask5Completed = false;
        private bool _isTask6Completed = false;
        private bool _isTask7Completed = false;

        public Main()
        {
            InitializeComponent();

            SetUserName();

            AddButton.Click += AddButton_Click;
            CompleteButton.Click += CompleteButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            TasksButton.Click += TasksButton_Click;
            HistoryButton.Click += HistoryButton_Click;

            HomeCategoryButton.Click += HomeCategoryButton_Click;
            WorkCategoryButton.Click += WorkCategoryButton_Click;
            StudyCategoryButton.Click += StudyCategoryButton_Click;
            RestCategoryButton.Click += RestCategoryButton_Click;

            Task1.MouseLeftButtonDown += TaskItem_Click;
            Task2.MouseLeftButtonDown += TaskItem_Click;
            Task3.MouseLeftButtonDown += TaskItem_Click;
            Task4.MouseLeftButtonDown += TaskItem_Click;
            Task5.MouseLeftButtonDown += TaskItem_Click;
            Task6.MouseLeftButtonDown += TaskItem_Click;
            Task7.MouseLeftButtonDown += TaskItem_Click;

            Task1Circle.MouseLeftButtonDown += TaskCircle_Click;
            Task2Circle.MouseLeftButtonDown += TaskCircle_Click;
            Task3Circle.MouseLeftButtonDown += TaskCircle_Click;
            Task4Circle.MouseLeftButtonDown += TaskCircle_Click;
            Task5Circle.MouseLeftButtonDown += TaskCircle_Click;
            Task6Circle.MouseLeftButtonDown += TaskCircle_Click;
            Task7Circle.MouseLeftButtonDown += TaskCircle_Click;

            Task1.MouseEnter += TaskItem_MouseEnter;
            Task2.MouseEnter += TaskItem_MouseEnter;
            Task3.MouseEnter += TaskItem_MouseEnter;
            Task4.MouseEnter += TaskItem_MouseEnter;
            Task5.MouseEnter += TaskItem_MouseEnter;
            Task6.MouseEnter += TaskItem_MouseEnter;
            Task7.MouseEnter += TaskItem_MouseEnter;

            Task1.MouseLeave += TaskItem_MouseLeave;
            Task2.MouseLeave += TaskItem_MouseLeave;
            Task3.MouseLeave += TaskItem_MouseLeave;
            Task4.MouseLeave += TaskItem_MouseLeave;
            Task5.MouseLeave += TaskItem_MouseLeave;
            Task6.MouseLeave += TaskItem_MouseLeave;
            Task7.MouseLeave += TaskItem_MouseLeave;

            UpdateTaskCompletionState();

            SelectTaskItem(Task1);
        }

       
        private void SetUserName()
        {
            if (CurrentUser.IsAuthenticated && !string.IsNullOrEmpty(CurrentUser.Username))
            {
                UserNameTextBlock.Text = CurrentUser.Username;
            }
            else
            {
                UserNameTextBlock.Text = "Гость";
            }
        }

        private void HomeCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            FilterTasksByCategory("Дом");
        }

        private void WorkCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            FilterTasksByCategory("Работа");
        }

        private void StudyCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            FilterTasksByCategory("Учеба");
        }

        private void RestCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            FilterTasksByCategory("Отдых");
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Показаны текущие задачи", "Задачи", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Показана история задач", "История", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FilterTasksByCategory(string category)
        {
            MessageBox.Show($"Показаны задачи категории: {category}",
                          "Фильтр",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void UpdateTaskCompletionState()
        {
            UpdateTaskState(Task1Circle, Task1Check, Task1Title, Task1Time, _isTask1Completed);
            UpdateTaskState(Task2Circle, Task2Check, Task2Title, Task2Time, _isTask2Completed);
            UpdateTaskState(Task3Circle, Task3Check, Task3Title, Task3Time, _isTask3Completed);
            UpdateTaskState(Task4Circle, Task4Check, Task4Title, Task4Time, _isTask4Completed);
            UpdateTaskState(Task5Circle, Task5Check, Task5Title, Task5Time, _isTask5Completed);
            UpdateTaskState(Task6Circle, Task6Check, Task6Title, Task6Time, _isTask6Completed);
            UpdateTaskState(Task7Circle, Task7Check, Task7Title, Task7Time, _isTask7Completed);
        }

        private void UpdateTaskState(Border circle, TextBlock check, TextBlock title, TextBlock time, bool isCompleted)
        {
            if (isCompleted)
            {
                circle.Background = new SolidColorBrush(Color.FromRgb(255, 107, 142));
                circle.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 107, 142));
                check.Visibility = Visibility.Visible;
                title.TextDecorations = TextDecorations.Strikethrough;
                title.Foreground = Brushes.Gray;
                time.Foreground = Brushes.Gray;
            }
            else
            {
                circle.Background = Brushes.White;
                circle.BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                check.Visibility = Visibility.Hidden;
                title.TextDecorations = null;
                title.Foreground = Brushes.Black;
                time.Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102));
            }
        }

        private void SelectTaskItem(Border taskItem)
        {
            if (_selectedTaskItem != null)
            {
                _selectedTaskItem.Background = Brushes.White;
            }

            _selectedTaskItem = taskItem;

            bool isCompleted = GetTaskCompletedStatus(taskItem);

            if (isCompleted)
            {
                _selectedTaskItem.Background = new SolidColorBrush(Color.FromArgb(20, 76, 175, 80));
            }
            else
            {
                _selectedTaskItem.Background = new SolidColorBrush(Color.FromArgb(20, 156, 39, 176));
            }

            UpdateTaskDetails(taskItem);
        }

        private bool GetTaskCompletedStatus(Border taskItem)
        {
            if (taskItem == Task1) return _isTask1Completed;
            if (taskItem == Task2) return _isTask2Completed;
            if (taskItem == Task3) return _isTask3Completed;
            if (taskItem == Task4) return _isTask4Completed;
            if (taskItem == Task5) return _isTask5Completed;
            if (taskItem == Task6) return _isTask6Completed;
            if (taskItem == Task7) return _isTask7Completed;
            return false;
        }

        private void UpdateTaskDetails(Border taskItem)
        {
            if (taskItem.Child is Grid grid)
            {
                if (grid.Children[1] is StackPanel stackPanel)
                {
                    if (stackPanel.Children[0] is TextBlock titleText)
                    {
                        TaskTitle.Text = titleText.Text;

                        if (stackPanel.Children[1] is TextBlock timeText)
                        {
                            TaskTime.Text = timeText.Text;
                        }

                        TaskDate.Text = "01 Января 2022";

                        if (taskItem == Task1)
                        {
                            TaskDescription.Text = "Встретиться со Стивеном у озера в 9 утра. Взять с собой рыболовные снасти, наживку и термос с горячим кофе. Обсудить планы на следующий рыболовный сезон.";
                        }
                        else if (taskItem == Task2)
                        {
                            TaskDescription.Text = "Прочитать автобиографию Златана Ибрагимовича. Обратить внимание на его подход к карьере и мотивации. Сделать заметки для личного развития.";
                        }
                        else if (taskItem == Task3)
                        {
                            TaskDescription.Text = "Купить молоко, хлеб, яйца, овощи и фрукты. Зайти в супермаркет после работы.";
                        }
                        else if (taskItem == Task4)
                        {
                            TaskDescription.Text = "Позвонить маме, узнать как дела, рассказать о последних событиях.";
                        }
                        else if (taskItem == Task5)
                        {
                            TaskDescription.Text = "Сходить в спортзал или сделать домашнюю тренировку. Упражнения на кардио и силовые.";
                        }
                        else if (taskItem == Task6)
                        {
                            TaskDescription.Text = "Проверить рабочую и личную почту. Ответить на срочные письма.";
                        }
                        else if (taskItem == Task7)
                        {
                            TaskDescription.Text = "Подготовить еженедельный отчет о проделанной работе для руководства.";
                        }
                    }
                }
            }
        }

        private void TaskItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                SelectTaskItem(border);
            }
        }

        private void TaskCircle_Click(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (sender is Border circle)
            {
                if (circle == Task1Circle) _isTask1Completed = !_isTask1Completed;
                else if (circle == Task2Circle) _isTask2Completed = !_isTask2Completed;
                else if (circle == Task3Circle) _isTask3Completed = !_isTask3Completed;
                else if (circle == Task4Circle) _isTask4Completed = !_isTask4Completed;
                else if (circle == Task5Circle) _isTask5Completed = !_isTask5Completed;
                else if (circle == Task6Circle) _isTask6Completed = !_isTask6Completed;
                else if (circle == Task7Circle) _isTask7Completed = !_isTask7Completed;

                UpdateTaskCompletionState();

                if (_selectedTaskItem != null)
                {
                    bool isCompleted = GetTaskCompletedStatus(_selectedTaskItem);

                    if (isCompleted)
                    {
                        _selectedTaskItem.Background = new SolidColorBrush(Color.FromArgb(20, 76, 175, 80));
                    }
                    else
                    {
                        _selectedTaskItem.Background = new SolidColorBrush(Color.FromArgb(20, 156, 39, 176));
                    }
                }
            }
        }

        private void TaskItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border && border != _selectedTaskItem)
            {
                border.Background = new SolidColorBrush(Color.FromArgb(10, 156, 39, 176));
            }
        }

        private void TaskItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border && border != _selectedTaskItem)
            {
                border.Background = Brushes.White;
            }
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTaskItem != null)
            {
                if (_selectedTaskItem == Task1) _isTask1Completed = true;
                else if (_selectedTaskItem == Task2) _isTask2Completed = true;
                else if (_selectedTaskItem == Task3) _isTask3Completed = true;
                else if (_selectedTaskItem == Task4) _isTask4Completed = true;
                else if (_selectedTaskItem == Task5) _isTask5Completed = true;
                else if (_selectedTaskItem == Task6) _isTask6Completed = true;
                else if (_selectedTaskItem == Task7) _isTask7Completed = true;

                UpdateTaskCompletionState();
                _selectedTaskItem.Background = new SolidColorBrush(Color.FromArgb(20, 76, 175, 80));

                MessageBox.Show("Задача отмечена как выполненная",
                              "Готово",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите задачу",
                              "Внимание",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTaskItem != null)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить задачу?",
                                                        "Удаление задачи",
                                                        MessageBoxButton.YesNo,
                                                        MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _selectedTaskItem.Visibility = Visibility.Collapsed;
                    _selectedTaskItem = null;

                    TaskTitle.Text = "Выберите задачу";
                    TaskTime.Text = "";
                    TaskDate.Text = "";
                    TaskDescription.Text = "";

                    MessageBox.Show("Задача удалена",
                                  "Удалено",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Выберите задачу",
                              "Внимание",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            CreateTaskWindow createTaskWindow = new CreateTaskWindow();
            createTaskWindow.Owner = this;
            createTaskWindow.ShowDialog();
        }
    }
}