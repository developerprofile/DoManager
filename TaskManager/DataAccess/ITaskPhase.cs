using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch.jaxx.TaskManager.DataAccess
{
    public interface ITaskPhase
    {
        int Id { get; set; }
        int TaskId { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
    }
}
