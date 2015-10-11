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

        /// <summary>
        /// Calculates the sum of all durations in the given ITaskPhase List but filters the liste between the two given dates.
        /// If one of the dates is null, the duration of the complete list will be calculated without appliying a filter.
        /// </summary>
        /// <param name="TaskPhasesList"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public TimeSpan GetTaskPhasesDuration(List<ITaskPhase> TaskPhasesList, DateTime? FromDate, DateTime? ToDate)
        {
            List<ITaskPhase> filteredList = new List<ITaskPhase>();
            if (FromDate.HasValue && ToDate.HasValue)
            {
                filteredList = TaskPhasesList.Where(p => p.EndDate.Value >= FromDate && p.EndDate < ToDate).ToList<ITaskPhase>();
            }
            else filteredList = TaskPhasesList;
            return GetTaskPhasesDuration(filteredList);            
        }
    }
}
