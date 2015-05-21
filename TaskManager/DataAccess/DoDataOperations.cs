using System;
using System.Collections.Generic;
using System.Linq;
using libjfunx.logging;
using System.Data.Entity;

namespace ch.jaxx.TaskManager.DataAccess
{
    public class DoDataOperations
    {
        private string connectionString;
        private FirebirdContext context;

        /// <summary>
        /// Creates a new instance of DoDataaOperations
        /// </summary>
        /// <param name="ConnectionString"></param>
        public DoDataOperations(string ConnectionString)
        {
            this.connectionString = ConnectionString;
            this.context = new FirebirdContext(this.connectionString);
        }

        public DoDataOperations()
        {
            this.context = new FirebirdContext();
        }

        /// <summary>
        /// Add a new task.
        /// </summary>
        /// <param name="TaskName">Taskname, task will no be created if taskname is empty</param>
        /// <returns>A task model of the created task, null if no task has been created.</returns>
        public TaskModel CreateTask(string Taskname)
        {
            // Don't do anything if taskname is not provided
            if (!String.IsNullOrWhiteSpace(Taskname))
            {
                var newTask = new TaskModel() { Name = Taskname, CreationDate = DateTime.Now };
                context.Tasks.Add(newTask);
                context.SaveChanges();
                return newTask;
            }
            return null;
        }


        /// <summary>
        /// Renames the task with the given task id.
        /// </summary>
        /// <param name="TaskId"></param>
        /// <param name="NewTaskName"></param>
        public void RenameTask(int TaskId, string NewTaskName)
        {
            var task = context.Tasks.Find(TaskId);
            if (task != null)
            {
                task.Name = NewTaskName;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Marks the task with the given Id as NEXT
        /// </summary>
        /// <param name="TaskId"></param>
        internal TaskModel MarkTaskAsNext(TaskModel nextTask)
        {
            if (nextTask != null)
            {
                ResetNextTask();
                nextTask.State = TaskState.NEXT;
                context.SaveChanges();
            }
            return nextTask;
        }

        /// <summary>
        /// Resets the NEXT task
        /// </summary>
        private void ResetNextTask()
        {
            if (NextTask != null)
            {
                NextTask.State = null;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Start a task.
        /// </summary>
        /// <param name="TaskToStart"></param>
        internal void StartTask(TaskModel TaskToStart)
        {
            var task = context.Tasks.Find(TaskToStart.Id);
            if (task.StartDate == null) task.StartDate = DateTime.Now;
            task.State = TaskState.ACTIVE;
            context.SaveChanges();
        }

        /// <summary>
        /// Stops a task (without setting it done)
        /// </summary>
        /// <param name="TaskToStop"></param>
        internal void StopTask(TaskModel TaskToStop)
        {
            var task = context.Tasks.Find(TaskToStop.Id);
            task.State = null;
            context.SaveChanges();
        }

        /// <summary>
        /// Stops and end a task by setting it's state to done and by setting
        /// an end date.
        /// </summary>
        /// <param name="TaskToStop"></param>
        internal void StopAndEndTask(TaskModel TaskToStop)
        {
            var task = context.Tasks.Find(TaskToStop.Id);
            task.State = TaskState.DONE;
            task.DoneDate = DateTime.Now;
            context.SaveChanges();
        }


        /// <summary>
        /// Creates a new phase for the given task.
        /// </summary>
        /// <param name="OwnerTask"></param>
        internal void StartTaskPhase(TaskModel OwnerTask)
        {
            var taskPhase = new TaskPhaseModel()
            {
                TaskId = OwnerTask.Id,
                StartDate = DateTime.Now
            };

            context.TaskPhases.Add(taskPhase);
            context.SaveChanges();
            Logger.Log(LogEintragTyp.Debug, "Start new task phase for taskid " + OwnerTask.Id);
        }

        /// <summary>
        /// Ends the active task phase of the owner task
        /// </summary>
        /// <param name="OwnerTask"></param>
        internal void EndTaskPhase(TaskModel OwnerTask)
        {
            var lastTaskPhase = context.TaskPhases.Where(p => p.TaskId == OwnerTask.Id)
                .Where(p => p.EndDate == null).FirstOrDefault();

            lastTaskPhase.EndDate = DateTime.Now;
            context.SaveChanges();
            Logger.Log(LogEintragTyp.Debug, "End task phase for taskid " + OwnerTask.Id);


        }

        /// <summary>
        /// Get the oldest task which is not done or active, returns null, if there isn't any.
        /// </summary>
        internal TaskModel OldestOpenTask
        {
            get
            {
                // find all task which are not done and not active
                var taskList = context.Tasks
                                    .Where(t => t.State != TaskState.DONE)
                                    .Where(t => t.State != TaskState.ACTIVE)
                                    .OrderBy(t => t.CreationDate);

                return taskList.FirstOrDefault();

            }
        }

        /// <summary>
        /// Gets the active task, null if no task is active.
        /// </summary>
        internal TaskModel ActiveTask
        {
            get
            {
                var activeTask = context.Tasks.Where(t => t.State == TaskState.ACTIVE).FirstOrDefault();
                if (activeTask != null)
                {
                    return activeTask;
                }
                else return null;
            }

        }

        /// <summary>
        /// Gets the next task, null if there is no next task.
        /// </summary>
        internal TaskModel NextTask
        {
            get
            {
                var nextTask = context.Tasks.Where(t => t.State == TaskState.NEXT).FirstOrDefault();
                if (nextTask != null)
                {
                    return nextTask;
                }
                else return null;
            }
        }

        /// <summary>
        /// Get an TaskModel by the given id, null if there is none
        /// </summary>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        internal TaskModel GetTaskById(int TaskId)
        {
            var task = context.Tasks.Where(t => t.Id == TaskId).FirstOrDefault();
            if (task != null)
            {
                return task;
            }
            else return null;
        }

        internal List<TaskModel> GetAllTasks
        {
            get
            {
                return context.Tasks.Where(t => t.State != TaskState.DONE).OrderByDescending(t => t.CreationDate).ToList();
            }
        }

        internal void LogTaskDurations(DateTime? Day = null)
        {
            // select all done tasks
            var doneTasks = context.Tasks.Where(t => t.State == TaskState.DONE);

            // filter by day if one is provided
            if (Day.HasValue)
            {
                doneTasks = doneTasks.Where(x => DbFunctions.TruncateTime(x.DoneDate.Value) == Day.Value);
            }

            // order the tasks by done date
            doneTasks.OrderBy(t => t.DoneDate);

            // foreach task which is done
            foreach (var task in doneTasks)
            {
                var taskDuration = new TimeSpan();
                var phases = context.TaskPhases.Where(p => p.TaskId == task.Id);

                // foreach phase of this task
                foreach (var phase in phases)
                {
                    var phaseduration = phase.EndDate.Value - phase.StartDate.Value;
                    taskDuration = taskDuration.Add(phaseduration);
                }

                Logger.Log(LogEintragTyp.Hinweis, String.Format("TaskDoneDate: {1} |  Duration {2} | Task: {0}", task.Name, task.DoneDate, taskDuration.ToString()));
            }

        }
    }
}
