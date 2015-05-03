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
            this.taskMan = new TaskManager(connectionString);
        }

        private string connectionString = @"server type=Embedded;user id=sysdba;password=masterky;dialect=3;character set=UTF8;client library=fbembed.dll;database=D:\task.fdb";
        private TaskManager taskMan;

        private void btnShowAllTasks_Click(object sender, RoutedEventArgs e)
        {            
            AllTasksToListBox();

        }

        private void btnMarkNextTask_Click(object sender, RoutedEventArgs e)
        {
            int nextTask = 0;
            Int32.TryParse(tbNextTask.Text, out nextTask);
            
            taskMan.MarkNextTask(nextTask);
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
            if (!String.IsNullOrWhiteSpace(tbNewTask.Text))
            taskMan.CreateQueueTask(tbNewTask.Text);
            tbNewTask.Text = "";
            AllTasksToListBox();
        }
        
    }
}
