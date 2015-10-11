using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch.jaxx.TaskManager.DataAccess
{
    public class TaskTaskPhaseConnector : ITaskTaskPhaseConnector
    {
        public List<ITaskPhase> MemberTaskPhases { get; set; }
        public ITask OwnerTask { get; set; }        
    }
}
