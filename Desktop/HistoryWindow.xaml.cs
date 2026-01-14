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
    public partial class HistoryWindow : Window
    {
        private Border _selectedTaskItem;
        private List<TaskModel> _completedTasks = new List<TaskModel>();
        private TaskRepository _taskRepository = new TaskRepository();
        private Dictionary<Border, TaskModel> _borderToTaskMap = new Dictionary<Border, TaskModel>();

        public HistoryWindow()
        {
            InitializeComponent();
            LoadCompletedTasks();
            DisplayCompletedTasks();
        }

        private void LoadCompletedTasks()
        {
            if (CurrentUser.IsAuthenticated && CurrentUser.User != null)
            {
                var allTasks = _taskRepository.GetUserTasks(CurrentUser.Username);
                _completedTasks = allTasks.Where(t => t.IsCompleted).OrderByDescending(t => t.Date).ToList();
            }
        }

        private void DisplayCompletedTasks()
        {
            HistoryStackPanel.Children.Clear();
            _borderToTaskMap.Clear();

            if (_completedTasks.Count == 0)
            {
                var messageText = new TextBlock
                {
                    Text = "Нет выполненных задач",
                    FontSize = 16,
                    Foreground = Brushes.Gray,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                };

                HistoryStackPanel.Children.Add(messageText);
                return;
            }

            foreach (var task in _completedTasks)
            {
                var taskBorder = CreateTaskBorder(task);
                HistoryStackPanel.Children.Add(taskBorder);
                _borderToTaskMap[taskBorder] = task;
            }

            if (_completedTasks.Count > 0)
            {
                var firstBorder = HistoryStackPanel.Children[0] as Border;
                if (firstBorder != null)
                {
                    SelectTaskItem(firstBorder);
                }
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

            var completedIndicator = new Border
            {
                Width = 24,
                Height = 24,
                CornerRadius = new CornerRadius(12),
                Margin = new Thickness(0, 0, 15, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush(Color.FromRgb(76, 175, 80))
            };

            var checkText = new TextBlock
            {
                Text = "✓",
                Foreground = Brushes.White,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            completedIndicator.Child = checkText;
            Grid.SetColumn(completedIndicator, 0);
            grid.Children.Add(completedIndicator);

            var stackPanel = new StackPanel();
            Grid.SetColumn(stackPanel, 1);
            grid.Children.Add(stackPanel);

            var titleText = new TextBlock
            {
                Text = task.Title,
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.Gray,
                TextDecorations = TextDecorations.Strikethrough
            };

            var timeText = new TextBlock
            {
                Text = $"{task.Time} ({task.Date.ToString("dd.MM.yyyy")})",
                FontSize = 12,
                Foreground = Brushes.Gray,
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
            _selectedTaskItem.Background = new SolidColorBrush(Color.FromArgb(20, 76, 175, 80));

            if (_borderToTaskMap.TryGetValue(taskItem, out TaskModel task))
            {
                UpdateTaskDetails(task);
            }
        }

        private void UpdateTaskDetails(TaskModel task)
        {
            HistoryTaskTitle.Text = task.Title;
            HistoryTaskTime.Text = task.Time;
            HistoryTaskDate.Text = task.Date.ToString("dd MMMM yyyy");
            HistoryTaskDescription.Text = task.Description;
        }

        private void TaskItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border && border != _selectedTaskItem)
            {
                border.Background = new SolidColorBrush(Color.FromArgb(10, 76, 175, 80));
            }
        }

        private void TaskItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border border && border != _selectedTaskItem)
            {
                border.Background = Brushes.White;
            }
        }
    }
}