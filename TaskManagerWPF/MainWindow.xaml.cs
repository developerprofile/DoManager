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

        private void btnShowAllTasks_Click(object sender, RoutedEventArgs e)
        {            
            AllTasksToListBox();

        }

        private void btnMarkNextTask_Click(object sender, RoutedEventArgs e)
        {  
            taskMan.MarkNextTask(tbNextTask.Text);
            tbNextTask.Text = "";
            AllTasksToListBox();

        }

        private void AllTasksToListBox()
        {
            var tasks = taskMan.GetAllTasks();
            listBoxTasks.Items.Clear();
            
            foreach (var t in tasks)
            {
                listBoxTasks.Items.Add(String.Format("TaskId: {0} \t Created: {1} \t TaskState: {2} \t {3}", t.Id, t.CreationDate, t.State, t.Name));
            }
        }

        private void btnStartNextTask_Click(object sender, RoutedEventArgs e)
        {
            taskMan.StartNextTask();
            AllTasksToListBox();
        }

        private void btnFinishTask_Click(object sender, RoutedEventArgs e)
        {
            taskMan.FinishCurrentTask();
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
            
            var openDialog = new Microsoft.Win32.OpenFileDialog();
            if(openDialog.ShowDialog().Value)
            {
                string connectionString = String.Format(@"server type=Embedded;user id=sysdba;password=masterky;dialect=3;character set=UTF8;client library=fbembed.dll;database={0}",openDialog.FileName);
                this.taskMan = new TaskManager(connectionString);
                btnOpenDb.IsEnabled = false;
                this.Title = this.Title + " - " + openDialog.FileName;
                
                AllTasksToListBox();
            }

            
        }

        private void btnLogDuration_Click(object sender, RoutedEventArgs e)
        {
            this.taskMan.LogTaskDurations();
        }
        
    }
}
