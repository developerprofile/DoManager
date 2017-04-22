using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ch.jaxx.TaskManager.DataAccess;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using System.Windows.Controls;
using Dapplo.LogFacade;
using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;

namespace DoManagerMui.ViewModel
{
    public class TaskListViewModel : ViewModelBase
    {
        private static readonly LogSource Log = new LogSource();

        private TaskManager taskMan;
        private MainWindow hostWindow;
        private IEnumerable<TaskViewModel> taskList;
        private string taskInputBox;



        public TaskListViewModel(TaskManager TaskManager, MainWindow HostWindow)
        {
            taskMan = TaskManager;
            hostWindow = HostWindow;
            hostWindow.Closing += HostWindow_Closing;
            RefreshTaskList();


            CreateNewTask = new RelayCommand<EventArgs>(ExecuteCreateNewTask, CanExecuteCreateNewTask);
            InterruptTask = new RelayCommand<EventArgs>(ExecuteInterruptTask, CanExecuteInterruptTask);
            LoadContextMenu = new RelayCommand<MouseButtonEventArgs>(ExecuteLoadContextMenu);
            StartNextTask = new RelayCommand<EventArgs>(ExecuteStartNextTask, CanExecuteStartNextTask);
            FinishTask = new RelayCommand<EventArgs>(ExecuteFinishTask, CanExecuteFinishTask);
            OnCellEditEnding = new RelayCommand<DataGridCellEditEndingEventArgs>(ExecuteOnCellEditEnding);
        }

        #region Bindings / Properties

        public string TaskInputBox
        {
            get { return taskInputBox;  }
            set
            {
                if (taskInputBox == value)
                    return;
                taskInputBox = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<TaskViewModel> TaskList
        {
            get
            {
                return taskList;
            }
            private set
            {
                if (taskList == value)
                    return;
                taskList = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ICommand

        public ICommand CreateNewTask { get; private set; }
        public ICommand LoadContextMenu { get; private set; }
        public ICommand StartNextTask { get; private set; }
        public ICommand FinishTask { get; private set; }
        public ICommand OnCellEditEnding { get; private set; }
        public ICommand InterruptTask { get; private set; }
        #endregion

        #region ExecuteCommands

        /// <summary>
        /// A task should only be created if there is a text in the input box
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanExecuteCreateNewTask(EventArgs obj)
        {
            return !(String.IsNullOrWhiteSpace(TaskInputBox));
        }

        private void ExecuteCreateNewTask(EventArgs obj)
        {
            taskMan.CreateQueueTask(TaskInputBox);
            TaskInputBox = "";
            RefreshTaskList();
        }

        private void ExecuteLoadContextMenu(MouseButtonEventArgs obj)
        {

            var source = obj.OriginalSource as TextBlock;
            // (#4) if user right click into datagrids header the cast to Textblock will fail, 
            //      source will be null and cause an ArgumentNullException when accessed
            if (source == null) return;
    
            var currentItem = source.DataContext as TaskViewModel;

            var contextMenu = new ContextMenu();

            var markNextMenuItem = new MenuItem { Header = "Mark As Next" };            

            markNextMenuItem.Click += (s, e) =>
            {
                taskMan.MarkNextTask(currentItem.Task.Id);
                RefreshTaskList();
            };

           
            contextMenu.Items.Add(markNextMenuItem);

            var blockMenuItem = new MenuItem { Header = "Block/Unblock" };
            blockMenuItem.Click += (s, e) =>
            {
                taskMan.BlockOrUnblockTask(currentItem.Task.Id);
                RefreshTaskList();
            };
            contextMenu.Items.Add(blockMenuItem);


            contextMenu.IsOpen = true;
        }

        /// <summary>
        /// A task can only be finished if there is an active task in the task list
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanExecuteFinishTask(EventArgs obj)
        {
            return taskMan.HasActiveTask;
        }

        private void ExecuteFinishTask(EventArgs obj)
        {
            taskMan.FinishCurrentTask();
            RefreshTaskList();
        }

        /// <summary>
        /// A next task could only be started if there is one ...
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanExecuteStartNextTask(EventArgs arg)
        {
            // If there is no task in state next we couldn't start it
            return taskMan.GetAllTasks().Where(t => t.State == TaskState.NEXT).Count() == 1;
        }

        private void ExecuteStartNextTask(EventArgs obj)
        {
            taskMan.StartNextTask();
            RefreshTaskList();
        }


        private void ExecuteOnCellEditEnding(DataGridCellEditEndingEventArgs obj)
        {
            if ((obj.EditingElement.Parent as DataGridCell).Column.Header.ToString() == "Name")
            {
                var editedTaskId = (obj.EditingElement.DataContext as TaskViewModel).Task.Id;
                var newValue = (obj.EditingElement as TextBox).Text;
                Log.Debug().WriteLine("OnCellEditEnding Taskid {0}, rename to {1}", editedTaskId, newValue);
                taskMan.RenameTask(editedTaskId, newValue);
            }

        }

        /// <summary>
        /// A task can just be interrupted if there is an active task and if the text input box is not empty
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanExecuteInterruptTask(EventArgs arg)
        {
           return !String.IsNullOrWhiteSpace(TaskInputBox) && taskMan.HasActiveTask;
        }

        private void ExecuteInterruptTask(EventArgs obj)
        {
            taskMan.InterruptCurrentTask(TaskInputBox);
            TaskInputBox = "";
            RefreshTaskList();
        }

        #endregion

        #region private methods

        private void RefreshTaskList()
        {
            var tasks = new List<TaskViewModel>();
            foreach(var t in taskMan.GetAllTasks())
            {
                var model = new TaskViewModel { Task = t };
                tasks.Add(model);
            }
            TaskList = tasks;

            RefreshOverlayIcon();
        }

        private void RefreshOverlayIcon()
        {
            if (taskMan.HasActiveTask) hostWindow.SetTaskBarIconActive();
            else hostWindow.SetTaskBarIconPaused();
        }

        private void HostWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // check for active task before closing the application
            if (taskMan != null && taskMan.HasActiveTask)
            {
                MessageBoxResult result = ModernDialog.ShowMessage("There is an active task.\r\nDo you really want to close the application?",
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

        #endregion
    }
}
