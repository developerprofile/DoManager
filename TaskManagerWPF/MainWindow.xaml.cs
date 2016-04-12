using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ch.jaxx.TaskManager.DataAccess;
using Dapplo.LogFacade;
using System.ComponentModel;

namespace TaskManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Dapplo.LogFacade.Loggers.NLogLogger.RegisterLogger(Dapplo.LogFacade.LogLevel.Verbose);
        }

        
        private TaskManager taskMan;

        private void btnMarkNextTask_Click(object sender, RoutedEventArgs e)
        {  
            taskMan.MarkNextTask(tbNextTask.Text);
            tbNextTask.Text = "";
            SetTaskBarIconPaused();
            AllTasksToListBox();

        }

        private void AllTasksToListBox()
        {
            // We have to ensure the persistence of column order and width (DOMA-6)
            // if there is already something in the grid save column order and width
            if (taskGrid.Columns.Count > 0)
            {
                ColumnOrder.SaveColumnOrder(taskGrid);
            }
            
            // Load or reload the tasks
            var tasks = taskMan.GetAllTasks();
            taskGrid.ItemsSource = tasks;

            // Reset column order and with if persisted before
            ColumnOrder.SetColumnOrder(taskGrid);
            
        }

        private void btnStartNextTask_Click(object sender, RoutedEventArgs e)
        {
            taskMan.StartNextTask();
            SetTaskBarIconActive();
            AllTasksToListBox();
        }

        private void btnFinishTask_Click(object sender, RoutedEventArgs e)
        {
            taskMan.FinishCurrentTask();
            SetTaskBarIconPaused();
            AllTasksToListBox();
        }

        private void btnCreateTask_Click(object sender, RoutedEventArgs e)
        {
            // DOMA-12: don't create a new task is text box is emtpy
            if (!String.IsNullOrWhiteSpace(tbNewTask.Text))
            {
                taskMan.CreateQueueTask(tbNewTask.Text);
                tbNewTask.Text = "";
                AllTasksToListBox();
            }
        }

        private void btnOpenDb_Click(object sender, RoutedEventArgs e)
        {
            // The MySql Style
            //this.taskMan = new TaskManager();
            //AllTasksToListBox();

            // The Firebird way
            var openDialog = new Microsoft.Win32.OpenFileDialog();
            if (openDialog.ShowDialog().Value)
            {
                string connectionString = String.Format(@"server type=Embedded;user id=sysdba;password=masterky;dialect=3;character set=UTF8;client library=fbembed.dll;database={0}", openDialog.FileName);
                this.taskMan = new TaskManager(connectionString);
                btnOpenDb.IsEnabled = false;
                this.Title = this.Title + " - " + openDialog.FileName;

                AllTasksToListBox();

                if (this.taskMan.HasActiveTask)
                {
                    SetTaskBarIconActive();
                }
                else SetTaskBarIconPaused();
            }

            
        }

        private void btnLogDuration_Click(object sender, RoutedEventArgs e)
        {
            var datePickerText = dtPicker.Text;
            DateTime parsedDateTime;

            if (DateTime.TryParse(datePickerText,out parsedDateTime))
            {
                this.taskMan.LogTaskDurations(parsedDateTime);
            }
            else this.taskMan.LogTaskDurations();
            
    }

        private void taskGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            var s = sender as DataGrid;
            var item = s.CurrentItem as TaskModel;
            tbNextTask.Text = item != null ? item.Id.ToString() : "";
        }

        private void btnInterrupt_Click(object sender, RoutedEventArgs e)
        {
            taskMan.InterruptCurrentTask(this.tbNewTask.Text);
            // Clear text field (DOMA-8)
            this.tbNewTask.Text = "";
            AllTasksToListBox();
        }

        private void taskGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if ((e.EditingElement.Parent as DataGridCell).Column.Header.ToString() == "Name")
            {
                var editedTaskId = (e.EditingElement.DataContext as TaskModel).Id;
                var newValue = (e.EditingElement as TextBox).Text;

                taskMan.RenameTask(editedTaskId, newValue);
            }
        }

        private void taskGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.IsReadOnly = true; // Makes the column as read only
            e.Column.CanUserReorder = true;
            e.Column.CanUserSort = true;            
           
            // Name column should be edidtable
            if (e.Column.Header.ToString() == "Name")
            {
                // e.Cancel = true;   // For not to include 
                e.Column.IsReadOnly = false; // Makes the column as read only
            }

        }

        private void SetTaskBarIconActive()
        {
            this.TaskbarItemInfo.Overlay = (ImageSource)Resources["OverlayImage"];
            this.TaskbarItemInfo.ProgressValue = 100;
            this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal; 
        }

        private void SetTaskBarIconPaused()
        {
            this.TaskbarItemInfo.Overlay = null;
            this.TaskbarItemInfo.ProgressValue = 100;
            this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // check for active task before closing the application (DOMA-10)
            if (taskMan != null && taskMan.HasActiveTask)
            {
                MessageBoxResult result = MessageBox.Show("There is an active task.\r\n Do you really want to close the application?",
                  "Confirmation", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // Yes code here
                    e.Cancel = false;
                }
                else if (result == MessageBoxResult.No)
                {
                    // No code here
                    e.Cancel = true;
                }
                else
                {
                    // Cancel code here
                    e.Cancel = true;
                }
            }
        }
        
    }
}
