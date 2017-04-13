using ch.jaxx.TaskManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoManagerMui.ViewModel
{
    public class TaskViewModel
    {
        public ITask Task { get; set; }

        public TaskMetaData MetaData
        {
            get
            {
                var meta = new TaskMetaData();
                switch (Task.State)
                {
                    case TaskState.ACTIVE:
                        meta.State = Task.State.ToString();
                        meta.Color = Color.OliveDrab.Name;
                        break;
                    case TaskState.BLOCKED:
                        meta.State = Task.State.ToString();
                        meta.Color = Color.Gray.Name;
                        break;
                    case TaskState.DONE:
                        meta.State = Task.State.ToString();
                        meta.Color = Color.Gray.Name;
                        break;
                    case TaskState.NEXT:
                        meta.State = Task.State.ToString();
                        meta.Color = Color.DarkOrange.Name;
                        break;
                    default:
                        meta.State = "TO DO";
                        meta.Color = Color.LightSalmon.Name;
                        break;
                }

                return meta;
            }
        }

       
    }

    public class TaskMetaData
    {
        public string Color { get; set; }
        public string State { get; set; }
    }
}
