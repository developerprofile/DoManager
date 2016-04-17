﻿using System;
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
        private IEnumerable<ITask> taskList;
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

        public IEnumerable<ITask> TaskList
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
            if (obj.Source == null) return;

            var source = obj.OriginalSource as TextBlock;
            var currentItem = source.DataContext as ITask;

            var contextMenu = new ContextMenu();

            var markNextMenuItem = new MenuItem { Header = "Mark as next" };
            markNextMenuItem.Click += (s, e) =>
            {
                taskMan.MarkNextTask(currentItem.Id);
                RefreshTaskList();
            };

           // contextMenu.Items.Add(new MenuItem { Header = currentItem.Id });
            contextMenu.Items.Add(markNextMenuItem);
           // contextMenu.Items.Add(new MenuItem { Header = "Item with gesture", InputGestureText = "Ctrl+C" });
           // contextMenu.Items.Add(new MenuItem { Header = "Item, disabled", IsEnabled = false });
           // contextMenu.Items.Add(new MenuItem { Header = "Item, checked", IsChecked = true });
           // contextMenu.Items.Add(new MenuItem { Header = "Item, checked and disabled", IsChecked = true, IsEnabled = false });
            //contextMenu.Items.Add(new Separator());
            //contextMenu.Items.Add(CreateSubMenu("Item with submenu"));

            //var menu = CreateSubMenu("Item with submenu, disabled");
            // menu.IsEnabled = false;
            //contextMenu.Items.Add(menu);

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
                var editedTaskId = (obj.EditingElement.DataContext as TaskModel).Id;
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
            TaskList = taskMan.GetAllTasks();
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