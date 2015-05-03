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
        /// <returns>Returns th task which has been marked as next and NULL in case the was
        /// no next task found.</returns>
        public TaskModel MarkNextTask()
        {
            var nextTask = context.Tasks
                                .Where(t => t.State != TaskState.DONE)
                                .OrderBy(t => t.CreationDate)
                                .FirstOrDefault();

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
        /// <returns>Returns the task wich has been started.</returns>
        public TaskModel StartNextTask()
        {
            var nextTask = context.Tasks.Where(t => t.State == TaskState.NEXT).FirstOrDefault();
            nextTask.State = TaskState.ACTIVE;
            nextTask.StartDate = DateTime.Now;
            context.SaveChanges();
            return nextTask;
        }

        /// <summary>
        /// Stops the task which is currently markes as ACTIVE.
        /// </summary>
        /// <returns>Returns the finished task, and NULL in case no active task was found.</returns>
        public TaskModel FinishCurrentTask()
        {
            var finishedTask = context.Tasks.Where(t => t.State == TaskState.ACTIVE).FirstOrDefault();
            if (finishedTask != null)
            {
                finishedTask.State = TaskState.DONE;
                finishedTask.DoneDate = DateTime.Now;
                context.SaveChanges();
                return finishedTask;
            }
            else return null;
        }

        public List<TaskModel> GetAllTasks()
        {
            return context.Tasks.ToList();            
        }
    }
}