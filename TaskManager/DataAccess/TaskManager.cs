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
        /// <param name="TaskToQueue"></param>
        public void CreateQueueTask(TaskModel TaskToQueue)
        {
            context.Tasks.Add(TaskToQueue);
            context.SaveChanges();
            
        }

        public List<TaskModel> GetAllTasks()
        {
            return context.Tasks.ToList();            
        }
    }
}