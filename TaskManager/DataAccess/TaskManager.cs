using System;
using System.Collections.Generic;
using Autofac;

namespace ch.jaxx.TaskManager.DataAccess
{
    public class TaskManager
    {
        private string connectionString;
        private DoDataOperations doDataOps;
        private IDoManagerSettings _settings;
        private static IContainer Container { get; set; }

        /// <summary>
        /// Creates a new instance of TaskManager
        /// </summary>
        /// <param name="ConnectionString"></param>
        public TaskManager(string ConnectionString, IDoManagerSettings Settings)
        {
            InitAutofacContainer();
            this.connectionString = ConnectionString;
            _settings = Settings; 
            this.doDataOps = new DoDataOperations(this.connectionString, _settings, Container);
        }

        private void InitAutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new TaskManagerConfiguratioModule());
            Container = builder.Build();
        }

        public TaskManager()
        {
            this.doDataOps = new DoDataOperations();
        }

        /// <summary>
        /// Adds a new task at the end of the task queue
        /// </summary>
        /// <param name="TaskName">Taskname, task will no be created if taskname is empty</param>
        /// <returns>A task model of the created task, null if no task has been created.</returns>
        public TaskModel CreateQueueTask(string TaskName)
        {
            var newTask = doDataOps.CreateTaskNow(TaskName);

            // if new task is also the oldest one, then it's the only one!
            // Make it next! (DOMA-7)
            SetNextIfOldestTask(newTask);

            return newTask;
        }

        /// <summary>
        /// If this task is the oldestOpenTask set it next
        /// </summary>
        /// <param name="task"></param>
        private void SetNextIfOldestTask(TaskModel task)
        {
            if (task.Id == doDataOps.OldestOpenTask.Id)
            {
                doDataOps.MarkTaskAsNext(task);
            }
        }


        /// <summary>
        /// Renames the task with the given task id.
        /// </summary>
        /// <param name="TaskId"></param>
        /// <param name="NewTaskName"></param>
        public void RenameTask(int TaskId, string NewTaskName)
        {
            doDataOps.RenameTask(TaskId, NewTaskName);
        }

        /// <summary>
        /// Evaluate the next task and mark it as NEXT
        /// </summary>
        /// <param name="TaskId">If provided the task with this id will become next task. Otherwise the oldest task will be selected
        /// (if 0 is passed, default = 0)</param>
        /// <returns>Returns the task which has been marked as next and NULL in case the was
        /// no next task found.</returns>
        /// <remarks>Passing the id of the active task will stop the current task phase and set the active task the next task.
        /// This is something like a pause funtion. When passing no task id, the active task will never become the next task.</remarks>
        public TaskModel MarkNextTask(int TaskId = 0)
        {
            // check if a special task id should become the next task,
            // otherwise choose the oldest task the next task
            TaskModel nextTask = null;
            var task = doDataOps.GetTaskById(TaskId);

            // if there is task switch and handle its state
            if (task != null)
            {
                switch (task.State)
                {
                    case null:
                        nextTask = doDataOps.MarkTaskAsNext(task);
                        break;
                    case TaskState.ACTIVE:
                        doDataOps.EndTaskPhaseNow(task);
                        nextTask = doDataOps.MarkTaskAsNext(task);
                        break;
                    case TaskState.NEXT:
                        nextTask = task;
                        break;
                    case TaskState.BLOCKED:
                        nextTask = doDataOps.MarkTaskAsNext(task);
                        break;
                    case TaskState.DONE:
                    default:
                        break;
                }
            }
            //else try to mark the oldest task
            else
            {
                var oldestTask = doDataOps.OldestOpenTask;
                nextTask = doDataOps.MarkTaskAsNext(oldestTask);
            }

            return nextTask;
        }

        /// <summary>
        /// Blocks or unblocks a task.
        /// </summary>
        /// <param name="id"></param>
        public void BlockOrUnblockTask(int id)
        {
            var task = doDataOps.GetTaskById(id);
            switch (task.State)
            {
                case null:
                    doDataOps.BlockTask(task);
                    break;
                case TaskState.BLOCKED:
                    doDataOps.ResetTaskState(task);
                    SetNextIfOldestTask(task);
                    break;
                case TaskState.ACTIVE:
                    doDataOps.EndTaskPhaseNow(task);
                    doDataOps.BlockTask(task);
                    break;
                case TaskState.NEXT:
                    doDataOps.BlockTask(task);
                    MarkNextTask();
                    break;
            }
        }

        /// <summary>
        /// Evaluate the next task an marks it as NEXT. This method is intended for passing taskid from UI Input fields where input 
        /// has not been validated as integer value.
        /// </summary>
        /// <param name="TaskId">TaskId</param>
        /// <returns>Returns th task which has been marked as next and NULL in case the was
        /// no next task found.</returns>
        /// <seealso cref="MarkNextTask(int TaskId)"/>
        /// <remarks>This method wrapps the MarkNextTask(int Taskid) but takes a string instead of an integer.
        /// The method will try to convert the string into integer and will pass 0 to to the MarkNextTask method. </remarks>
        public TaskModel MarkNextTask(string TaskId)
        {
            int nextTask = 0;
            Int32.TryParse(TaskId, out nextTask);
            return MarkNextTask(nextTask);
        }

        /// <summary>
        /// Starts the next task by setting it's start date to now. After starting this function will mark the next task.
        /// </summary>
        /// <returns>Returns the task wich has been started and null, if no task to start was found. 
        /// If a task is active this function will toggle the active state between active and next task.
        ///</returns>
        public TaskModel StartNextTask()
        {
            var activeTask = doDataOps.ActiveTask;
            var nextTask = doDataOps.NextTask;
            if (nextTask != null)
            {
                doDataOps.StartTaskNow(nextTask);
                doDataOps.StartTaskPhaseNow(nextTask);
                if (activeTask != null)
                {
                    doDataOps.MarkTaskAsNext(activeTask);
                    doDataOps.EndTaskPhaseNow(activeTask);
                }
                else doDataOps.MarkTaskAsNext(doDataOps.OldestOpenTask);
                return nextTask;
            }
            else return null;
        }

        /// <summary>
        /// If there is an active task this method will create a new task and start it. Thi will interrupt the active task immediately.
        /// The interrupted task will become the new next task.
        /// </summary>
        /// <param name="TaskName">Name of new task, if empty nothing will happen.</param>
        public void InterruptCurrentTask(string TaskName)
        {
            var interruptedTask = doDataOps.ActiveTask;
            // to interrupt the active task, there must be an active task
            if (interruptedTask != null && !String.IsNullOrEmpty(TaskName))
            {
                // Create new task
                var newTask = CreateQueueTask(TaskName);

                // Stop the current active task phase
                doDataOps.EndTaskPhaseNow(interruptedTask);
                // Stop the current active task
                doDataOps.ResetTaskState(interruptedTask);

                // Mark new task next as next to interrupt it
                doDataOps.MarkTaskAsNext(newTask);
                // Start the next task
                doDataOps.StartTaskNow(newTask);
                // Start new task phase
                doDataOps.StartTaskPhaseNow(newTask);

                // and mark interrupted task as new next task 
                doDataOps.MarkTaskAsNext(interruptedTask);

            }
        }

        /// <summary>
        /// Stops the task which is currently markes as ACTIVE.
        /// </summary>
        /// <returns>Returns the finished task, and NULL in case no active task was found.</returns>
        public TaskModel FinishCurrentTask()
        {
            var finishedTask = doDataOps.ActiveTask;
            if (finishedTask != null)
            {
                doDataOps.EndTaskPhaseNow(finishedTask);
                doDataOps.StopAndEndTaskNow(finishedTask);
                return finishedTask;
            }
            else return null;
        }

        /// <summary>
        /// Gets all tasks not done.
        /// </summary>
        /// <returns></returns>
        public List<TaskModel> GetAllTasks()
        {
            return doDataOps.GetAllTasks;
        }

        public void LogTaskDurations(DateTime? Day = null)
        {
            doDataOps.LogTaskDurations(Day);
        }

        /// <summary>
        /// Gets an indicator, wether the current task list has an active task.
        /// </summary>
        public bool HasActiveTask
        {
            get
            {
                return doDataOps.ActiveTask != null;
            }
        }

        public IEnumerable<string> GetTaskPhaseReport(DateTime? Day = null)
        {
            var result = doDataOps.GetTaskPhaseList(Day);
            return result;
        }
    }
}