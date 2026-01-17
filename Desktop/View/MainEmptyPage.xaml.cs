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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            
            if (NavigationService != null)
            {
                NavigationService.Navigate(new Uri("/View/MainWindow.xaml", UriKind.Relative));
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            
            if (NavigationService != null)
            {
                NavigationService.Navigate(new CreateTaskPage());
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (CurrentUser.IsAuthenticated && CurrentUser.User != null)
            {
                var taskRepository = new TaskRepository();
                bool hasTasks = taskRepository.UserHasTasks(CurrentUser.User.Username);

                if (hasTasks)
                {
                    
                    if (NavigationService != null)
                    {
                        NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
                    }
                }
            }
        }
    }
}