using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch.jaxx.TaskManager.DataAccess
{
    /// <summary>
    /// This interface define methods to generate time reports.
    /// </summary>
    public interface ITimeReport
    {
        /// <summary>
        /// Returns a report list for the given TaskTaskPhaseConnectorList and dates.
        /// </summary>
        /// <param name="TaskTaskPhaseConnectors"></param>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <returns></returns>
        List<string> ReportList(List<ITaskTaskPhaseConnector> TaskTaskPhaseConnectors, DateTime? FromDate, DateTime? ToDate);

        /// <summary>
        /// Writes the report into a file.
        /// </summary>
        /// <param name="FileName"></param>
        void WriteToFile(string FileName);
    }
}