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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ch.jaxx.TaskManager.DataAccess;
using libjfunx.logging;
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
            Logger.SetLogger(new ReflectingFileLogger(Properties.Settings.Default.Logfile,LogEintragTyp.Debug));            
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
            taskMan.CreateQueueTask(tbNewTask.Text);
            tbNewTask.Text = "";
            AllTasksToListBox();
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
        
    }
}
