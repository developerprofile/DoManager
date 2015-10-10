using System;
using System.Collections.Generic;


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

        /// <summary>
        /// Calculates duration of the task phase.
        /// </summary>
        /// <param name="TaskPhase"></param>
        /// <returns></returns>
        TimeSpan GetTaskPhaseDuration(ITaskPhase TaskPhase);

        /// <summary>
        /// Calculates the sum of all durations in the given ITaskPhase List.
        /// </summary>
        /// <param name="TaskPhasesList"></param>
        /// <returns></returns>
        TimeSpan GetTaskPhasesDuration(List<ITaskPhase> TaskPhasesList);

        /// <summary>
        /// Calculates the sum of all durations in the given ITaskPhase List but filters the liste between the two given dates.
        /// </summary>
        /// <param name="TaskPhasesList"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        TimeSpan GetTaskPhasesDuration(List<ITaskPhase> TaskPhasesList, DateTime FromDate, DateTime ToDate);
    }
}
