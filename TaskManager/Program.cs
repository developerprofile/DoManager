using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ch.jaxx.TaskManager.DataAccess;

namespace ch.jaxx.TaskManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new TestAccess();
            test.GetTasks();
        }
    }
}
