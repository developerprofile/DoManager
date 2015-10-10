using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch.jaxx.TaskManager.DataAccess
{
    public interface ITask
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime? CreationDate { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? DoneDate { get; set; }
        TaskState? State { get; set; }
    }
}
