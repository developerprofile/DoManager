using System;
using System.Collections.Generic;
using System.Linq;
using libjfunx.logging;

namespace ch.jaxx.TaskManager.DataAccess
{
    public class TaskManager
    {
        private string connectionString;
        private FirebirdContext context;

        /// <summary>
        /// Creates a new instance of TaskManager
        /// </summary>
        /// <param name="ConnectionString"></param>
        public TaskManager(string ConnectionString)
        {
            this.connectionString = ConnectionString;
            this.context = new FirebirdContext(this.connectionString);
        }

        /// <summary>
        /// Adds a new task at the end of the task queue
        /// </summary>
        /// <param name="TaskName">Taskname, task will no be created if taskname is empty</param>
        public void CreateQueueTask(string TaskName)
        {
            // Don't do anything if taskname is not provided
            if (!String.IsNullOrWhiteSpace(TaskName))
            {
                context.Tasks.Add(new TaskModel() { Name = TaskName, CreationDate = DateTime.Now });
                context.SaveChanges();
            }
            
        }

        /// <summary>
        /// Evaluate the next task and mark it as NEXT
        /// </summary>
        /// <param name="TaskId">If provided the task with this id will become next task. Otherwise the oldest task will be selected
        /// (if 0 is passed, default = 0)</param>
        /// <returns>Returns th task which has been marked as next and NULL in case the was
        /// no next task found.</returns>
        public TaskModel MarkNextTask(int TaskId = 0) 
        {
            // if a next tasks exists, first reset it' state
            var resetFormerlyNext = context.Tasks.Where(t => t.State == TaskState.NEXT).FirstOrDefault();
            if (resetFormerlyNext != null) resetFormerlyNext.State = null;

            // find all task which are not done and not active
            var taskList = context.Tasks
                                .Where(t => t.State != TaskState.DONE)
                                //.Where(t => t.State != TaskState.ACTIVE) 
                                // TODO: Once interupt is handled remove comment
                                // > as long as interupt is not handled we've to keep a chance to reset an active task to NEXT state
                                .OrderBy(t => t.CreationDate);

            // check if a special task id should become the next task,
            // otherwise choose the oldest task the next task
            TaskModel nextTask = null;
            if (TaskId != 0)
            {
                nextTask = taskList.Where(t => t.Id == TaskId).FirstOrDefault();
            }
            // Is nextTask still null? So TaskId is 0 or
            // maybe the given task id was wrong and was not in list,
            // so we'll give taskman a chance to mark the oldest task as next
            if (nextTask == null) nextTask = taskList.Where(t => t.State != TaskState.ACTIVE).FirstOrDefault();
            // TODO: Once interupt is handled filter ACTIVE state as described some lines above.
            

            // maybe there is no task at all in our list, so return null
            // otherwise make the selected task the next task
            if (nextTask != null)
            {
                // if next task is the active one, pause it
                if (nextTask.State == TaskState.ACTIVE) EndTaskPhase(ActiveTask);
                nextTask.State = TaskState.NEXT;
                context.SaveChanges();
                return nextTask;
            }
            else return null;
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
        /// Starts the next task by setting it's start date to now.
        /// </summary>
        /// <returns>Returns the task wich has been started and null, if no task to start was found or there is still 
        /// an unfinished task.</returns>
        public TaskModel StartNextTask()
        {
            if (ActiveTask != null) return null;
            var nextTask = context.Tasks.Where(t => t.State == TaskState.NEXT).FirstOrDefault();
            if (nextTask != null)
            {
                nextTask.State = TaskState.ACTIVE;
                if (nextTask.StartDate == null) nextTask.StartDate = DateTime.Now;
                context.SaveChanges();
                StartTaskPhase(nextTask);
                return nextTask;
            }
            else return null;
        }


        /// <summary>
        /// Stops the task which is currently markes as ACTIVE.
        /// </summary>
        /// <returns>Returns the finished task, and NULL in case no active task was found.</returns>
        public TaskModel FinishCurrentTask()
        {
            var finishedTask = ActiveTask;
            if (finishedTask != null)
            {
                EndTaskPhase(finishedTask);

                finishedTask.State = TaskState.DONE;
                finishedTask.DoneDate = DateTime.Now;
                context.SaveChanges();
                return finishedTask;
            }
            else return null;
        }


        /// <summary>
        /// Gets the active task, null if no task is active.
        /// </summary>
        public TaskModel ActiveTask
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
        /// Creates a new phase for the given task.
        /// </summary>
        /// <param name="OwnerTask"></param>
        private void StartTaskPhase(TaskModel OwnerTask)
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
        private void EndTaskPhase(TaskModel OwnerTask)
        {
            var lastTaskPhase = context.TaskPhases.Where(p => p.TaskId == OwnerTask.Id)
                .Where(p => p.EndDate == null).FirstOrDefault();

            lastTaskPhase.EndDate = DateTime.Now;
            context.SaveChanges();
            Logger.Log(LogEintragTyp.Debug, "End task phase for taskid " + OwnerTask.Id);

            
        }

        public List<TaskModel> GetAllTasks()
        {
            return context.Tasks.Where(t => t.State != TaskState.DONE).ToList();            
        }
    }
}