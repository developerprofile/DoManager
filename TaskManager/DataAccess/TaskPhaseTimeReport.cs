using System;
using System.Collections.Generic;
using System.IO;

namespace ch.jaxx.TaskManager.DataAccess
{
    /// <summary>
    /// This class provides methods to generate time reports.
    /// </summary>
    public class TaskPhaseTimeReport : ITimeReport
    {
        private ITimeReporter timeReporter;
        private List<string> report;

        /// <summary>
        /// Constructor, which takes a ITimeReporter which delivers the time calculations.
        /// </summary>
        /// <param name="reporter"></param>
        public TaskPhaseTimeReport(ITimeReporter reporter)
        {
            timeReporter = reporter;
        }

        /// <summary>
        /// Returns a report list for the given TaskTaskPhaseConnectorList and dates.
        /// </summary>
        /// <param name="TaskTaskPhaseConnectors"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        public List<string> ReportList(List<ITaskTaskPhaseConnector> TaskTaskPhaseConnectors, DateTime? FromDate, DateTime? ToDate)
        {
            var resultList = new List<string>();
            foreach (var task in TaskTaskPhaseConnectors)
            {
                var duration = timeReporter.GetTaskPhasesDuration(task.MemberTaskPhases, FromDate, ToDate);
                resultList.Add(String.Format("Task: {0}, PhasesDuration: {1}", task.OwnerTask.Name, duration.ToString()));
            }
            report = resultList;
            return report;
        }

        /// <summary>
        /// Writes the report into a file.
        /// </summary>
        /// <param name="FileName"></param>
        public void WriteToFile(string FileName)
        {
            Directory.CreateDirectory(new FileInfo(FileName).DirectoryName);
            File.WriteAllLines(FileName, report);
        }
    }
}
