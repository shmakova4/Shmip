using Desktop.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Todo.Entities;

namespace Desktop
{
    public partial class Main : Window
    {
        private Border _selectedTaskItem;
        private List<TaskModel> _userTasks = new List<TaskModel>();
        private TaskRepository _taskRepository = new TaskRepository();
        private Dictionary<Border, TaskModel> _borderToTaskMap = new Dictionary<Border, TaskModel>();

        public Main()
        {
            InitializeComponent();
            SetUserName();
            LoadUserTasks();
            InitializeEventHandlers();
            DisplayUserTasks();
        }

        private void InitializeEventHandlers()
        {
            AddButton.Click += AddButton_Click;
            CompleteButton.Click += CompleteButton_Click;
            DeleteButton.Click += DeleteButton_Click;
            TasksButton.Click += TasksButton_Click;
            HistoryButton.Click += HistoryButton_Click;

            HomeCategoryButton.Click += HomeCategoryButton_Click;
            WorkCategoryButton.Click += WorkCategoryButton_Click;
            StudyCategoryButton.Click += StudyCategoryButton_Click;
            RestCategoryButton.Click += RestCategoryButton_Click;
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

        private void LoadUserTasks()
        {
            if (CurrentUser.IsAuthenticated && CurrentUser.User != null)
            {
                _userTasks = _taskRepository.GetUserTasks(CurrentUser.Username);
            }
        }

        private void DisplayUserTasks()
        {
            TasksStackPanel.Children.Clear();
            _borderToTaskMap.Clear();

            foreach (var task in _userTasks)
            {
                var taskBorder = CreateTaskBorder(task);
                TasksStackPanel.Children.Add(taskBorder);
                _borderToTaskMap[taskBorder] = task;
            }

            if (_userTasks.Count > 0)
            {
                var firstBorder = TasksStackPanel.Children[0] as Border;
                if (firstBorder != null)
                {
                    SelectTaskItem(firstBorder);
                }
            }
            else
            {
                TaskTitle.Text = "Нет задач";
                TaskTime.Text = "";
                TaskDate.Text = "";
                TaskDescription.Text = "Нажмите кнопку + чтобы создать первую задачу";
            }
        }

        private Border CreateTaskBorder(TaskModel task)
        {
            var border = new Border
            {
                Background = Brushes.White,
                BorderThickness = new Thickness(0, 0, 0, 1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                Padding = new Thickness(15, 15, 15, 15),
                Margin = new Thickness(0, 0, 0, 0),
                Tag = task
            };

            border.MouseLeftButtonDown += TaskItem_Click;
            border.MouseEnter += TaskItem_MouseEnter;
            border.MouseLeave += TaskItem_MouseLeave;

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var circleBorder = new Border
            {
                Width = 24,
                Height = 24,
                CornerRadius = new CornerRadius(12),
                BorderThickness = new Thickness(2),
                BorderBrush = task.IsCompleted ?
                    new SolidColorBrush(Color.FromRgb(255, 107, 142)) :
                    new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                Margin = new Thickness(0, 0, 15, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Background = task.IsCompleted ?
                    new SolidColorBrush(Color.FromRgb(255, 107, 142)) :
                    Brushes.White
            };

            circleBorder.MouseLeftButtonDown += TaskCircle_Click;

            var checkText = new TextBlock
            {
                Text = "✓",
                Foreground = Brushes.White,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = task.IsCompleted ? Visibility.Visible : Visibility.Hidden
            };

            circleBorder.Child = checkText;
            Grid.SetColumn(circleBorder, 0);
            grid.Children.Add(circleBorder);

            var stackPanel = new StackPanel();
            Grid.SetColumn(stackPanel, 1);
            grid.Children.Add(stackPanel);

            var titleText = new TextBlock
            {
                Text = task.Title,
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Foreground = task.IsCompleted ? Brushes.Gray : Brushes.Black,
                TextDecorations = task.IsCompleted ? TextDecorations.Strikethrough : null
            };

            var timeText = new TextBlock
            {
                Text = task.Time,
                FontSize = 12,
                Foreground = task.IsCompleted ? Brushes.Gray : new SolidColorBrush(Color.FromRgb(102, 102, 102)),
                Margin = new Thickness(0, 5, 0, 0)
            };

            stackPanel.Children.Add(titleText);
            stackPanel.Children.Add(timeText);

            border.Child = grid;
            return border;
        }

        private void TaskItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                SelectTaskItem(border);
            }
        }

        private void SelectTaskItem(Border taskItem)
        {
            if (_selectedTaskItem != null)
            {
                _selectedTaskItem.Background = Brushes.White;
            }

            _selectedTaskItem = taskItem;

            if (_borderToTaskMap.TryGetValue(taskItem, out TaskModel task))
            {
                if (task.IsCompleted)
                {
                    _selectedTaskItem.Background = new SolidColorBrush(Color.FromArgb(20, 76, 175, 80));
                }
                else
                {
                    _selectedTaskItem.Background = new SolidColorBrush(Color.FromArgb(20, 156, 39, 176));
                }

                UpdateTaskDetails(task);
            }
        }

        private void UpdateTaskDetails(TaskModel task)
        {
            TaskTitle.Text = task.Title;
            TaskTime.Text = task.Time;
            TaskDate.Text = task.Date.ToString("dd MMMM yyyy");
            TaskDescription.Text = task.Description;
        }

        private void TaskCircle_Click(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (sender is Border circle && circle.Parent is Grid grid)
            {
                var taskBorder = FindParentBorder(circle);
                if (taskBorder != null && _borderToTaskMap.TryGetValue(taskBorder, out TaskModel task))
                {
                    task.IsCompleted = !task.IsCompleted;
                    _taskRepository.UpdateTask(task);

                    var checkText = circle.Child as TextBlock;
                    if (checkText != null)
                    {
                        checkText.Visibility = task.IsCompleted ? Visibility.Visible : Visibility.Hidden;
                    }

                    circle.Background = task.IsCompleted ?
                        new SolidColorBrush(Color.FromRgb(255, 107, 142)) :
                        Brushes.White;
                    circle.BorderBrush = task.IsCompleted ?
                        new SolidColorBrush(Color.FromRgb(255, 107, 142)) :
                        new SolidColorBrush(Color.FromRgb(224, 224, 224));

                    if (grid.Children[1] is StackPanel stackPanel)
                    {
                        if (stackPanel.Children[0] is TextBlock titleText)
                        {
                            titleText.Foreground = task.IsCompleted ? Brushes.Gray : Brushes.Black;
                            titleText.TextDecorations = task.IsCompleted ? TextDecorations.Strikethrough : null;
                        }

                        if (stackPanel.Children[1] is TextBlock timeText)
                        {
                            timeText.Foreground = task.IsCompleted ? Brushes.Gray : new SolidColorBrush(Color.FromRgb(102, 102, 102));
                        }
                    }

                    if (_selectedTaskItem == taskBorder)
                    {
                        if (task.IsCompleted)
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
        }

        private Border FindParentBorder(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is Border))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as Border;
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
            if (_selectedTaskItem != null && _borderToTaskMap.TryGetValue(_selectedTaskItem, out TaskModel task))
            {
                if (!task.IsCompleted)
                {
                    task.IsCompleted = true;
                    _taskRepository.UpdateTask(task);
                    DisplayUserTasks();

                    MessageBox.Show("Задача отмечена как выполненная",
                                  "Готово",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Задача уже выполнена",
                                  "Внимание",
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTaskItem != null && _borderToTaskMap.TryGetValue(_selectedTaskItem, out TaskModel task))
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить задачу?",
                                                        "Удаление задачи",
                                                        MessageBoxButton.YesNo,
                                                        MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool deleted = _taskRepository.DeleteTask(task.Username, task.Id);
                    if (deleted)
                    {
                        _userTasks.Remove(task);
                        _borderToTaskMap.Remove(_selectedTaskItem);
                        TasksStackPanel.Children.Remove(_selectedTaskItem);

                        TaskTitle.Text = "Выберите задачу";
                        TaskTime.Text = "";
                        TaskDate.Text = "";
                        TaskDescription.Text = "";

                        _selectedTaskItem = null;

                        MessageBox.Show("Задача удалена",
                                      "Удалено",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении задачи",
                                      "Ошибка",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Error);
                    }
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

            if (createTaskWindow.ShowDialog() == true)
            {
                LoadUserTasks();
                DisplayUserTasks();
            }
        }

        private void TasksButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserTasks();
            DisplayUserTasks();
            MessageBox.Show("Задачи обновлены", "Задачи", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var completedTasks = _userTasks.Where(t => t.IsCompleted).ToList();
            MessageBox.Show($"Выполнено задач: {completedTasks.Count}",
                          "История",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
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

        private void FilterTasksByCategory(string category)
        {
            var filteredTasks = _userTasks.Where(t => t.Category == category).ToList();

            TasksStackPanel.Children.Clear();
            _borderToTaskMap.Clear();

            foreach (var task in filteredTasks)
            {
                var taskBorder = CreateTaskBorder(task);
                TasksStackPanel.Children.Add(taskBorder);
                _borderToTaskMap[taskBorder] = task;
            }

            MessageBox.Show($"Показаны задачи категории: {category} (всего: {filteredTasks.Count})",
                          "Фильтр",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            LoadUserTasks();
            DisplayUserTasks();
        }
    }
}