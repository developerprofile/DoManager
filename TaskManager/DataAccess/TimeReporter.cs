using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch.jaxx.TaskManager.DataAccess
{
    public class TimeReporter : ITimeReporter
    {
        private TimeSpan CalculateTimeDifference(DateTime? StartDate, DateTime? EndDate)
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                return EndDate.Value - StartDate.Value;
            }
            else throw new ArgumentNullException();
        }

        /// <summary>
        /// Calculate the whole duration of a task from start to done date.
        /// Task phases, phase durations and pauses are disregarded. 
        /// </summary>
        /// <param name="Task"></param>
        /// <returns></returns>
        public TimeSpan GetTaskDuration(ITask Task)
        {
           return CalculateTimeDifference(Task.StartDate, Task.DoneDate);
        }

        /// <summary>
        /// Calculates duration of the task phase
        /// </summary>
        /// <param name="TaskPhase"></param>
        /// <returns></returns>
        public TimeSpan GetTaskPhaseDuration(ITaskPhase TaskPhase)
        {
            return CalculateTimeDifference(TaskPhase.StartDate, TaskPhase.EndDate);
        }

        /// <summary>
        /// Calculates the sum of all durations in the given ITaskPhase List.
        /// </summary>
        /// <param name="TaskPhasesList"></param>
        /// <returns></returns>
        public TimeSpan GetTaskPhasesDuration(List<ITaskPhase> TaskPhasesList)
        {
            TimeSpan duration = new TimeSpan(0);
            foreach (var phase in TaskPhasesList)
            {
                var phaseDuration = GetTaskPhaseDuration(phase);
                duration = duration.Add(phaseDuration);
            }
            return duration;
        }
    }
}
