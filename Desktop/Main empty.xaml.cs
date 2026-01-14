using Desktop.Repository;
using System;
using System.Windows;
using Todo.Entities;

namespace Desktop
{
    public partial class Main_empty : Window
    {
        public Main_empty()
        {
            InitializeComponent();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Close();
            var loginWindow = new MainWindow();
            loginWindow.Show();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            CreateTaskWindow createTaskWindow = new CreateTaskWindow();
            createTaskWindow.Owner = this;
            bool? result = createTaskWindow.ShowDialog();

            if (result == true)
            {
                Main mainWindow = new Main();
                mainWindow.Show();
                this.Close();
            }
        }

        private void CreateTaskWindow_Closed(object sender, EventArgs e)
        {
            
            if (CurrentUser.IsAuthenticated && CurrentUser.User != null)
            {
                var taskRepository = new TaskRepository();
                bool hasTasks = taskRepository.UserHasTasks(CurrentUser.User.Username);

                if (hasTasks)
                {
                    Main mainWindow = new Main();
                    mainWindow.Show();
                    this.Close();
                }
            }
        }
    }
}