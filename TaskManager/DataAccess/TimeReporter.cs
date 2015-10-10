using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch.jaxx.TaskManager.DataAccess
{
    public class TimeReporter : ITimeReporter
    {
        public TimeSpan GetTaskDuration(ITask Task)
        {
            throw new NotImplementedException();
        }
    }
}
