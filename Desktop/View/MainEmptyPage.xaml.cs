using Desktop.Repository;
using System;
using System.Windows;
using System.Windows.Controls;
using Todo.Entities;

namespace Desktop.View
{
    public partial class MainEmptyPage : Page
    {
        public MainEmptyPage()
        {
            InitializeComponent();
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                await mainWindow.NavigateToPageAsync(new CreateTaskPage());
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.IsAuthenticated && CurrentUser.User != null)
            {
                var taskRepository = new TaskRepository();
                bool hasTasks = taskRepository.UserHasTasks(CurrentUser.User.Username);

                if (hasTasks)
                {
                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        await mainWindow.NavigateToPageAsync(new MainPage());
                    }
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.NavigateBackToLogin();
            }
        }
    }
}