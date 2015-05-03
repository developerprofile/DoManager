using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="TaskName"/>
        public void CreateQueueTask(string TaskName)
        {
            context.Tasks.Add(new TaskModel() { Name = TaskName, CreationDate = DateTime.Now });
            context.SaveChanges();
            
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
                                .Where(t => t.State != TaskState.ACTIVE)
                                .OrderBy(t => t.CreationDate);

            // check if a special task id should become the next task,
            // otherwise choose the oldest task the next task
            TaskModel nextTask;
            if (TaskId != 0)
            {
                nextTask = taskList.Where(t => t.Id == TaskId).FirstOrDefault();
            }
            else nextTask = taskList.FirstOrDefault();

            // maybe there is no task at all in our list, so return null
            // otherwise make the selected task the next task
            if (nextTask != null)
            {
                nextTask.State = TaskState.NEXT;
                context.SaveChanges();
                return nextTask;
            }
            else return null;
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
                nextTask.StartDate = DateTime.Now;
                context.SaveChanges();
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

        public List<TaskModel> GetAllTasks()
        {
            return context.Tasks.Where(t => t.State != TaskState.DONE).ToList();            
        }
    }
}