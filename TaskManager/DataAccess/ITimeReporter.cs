using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch.jaxx.TaskManager.DataAccess
{
    public interface ITimeReporter
    {
        /// <summary>
        /// Calculate the whole duration of a task from start to done date.
        /// Task phases, phase durations and pauses are disregarded. 
        /// </summary>
        /// <param name="Task"></param>
        /// <returns></returns>
        TimeSpan GetTaskDuration(ITask Task);
    }
}
