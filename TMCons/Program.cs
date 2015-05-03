using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ch.jaxx.TaskManager.DataAccess;

namespace TMCons
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"server type=Embedded;user id=sysdba;password=masterky;dialect=3;character set=UTF8;client library=fbembed.dll;database=D:\task.fdb";
            var taskMan = new TaskManager(connectionString);
            Console.Write("Input new task: ");
            var input = Console.ReadLine();
            Console.WriteLine("Insert entry...");
            taskMan.CreateQueueTask(input);
            Console.WriteLine("List tasks...");
            foreach (var t in taskMan.GetAllTasks())
            {
                Console.WriteLine("Taskid: {0} \t Created: {2} \t Taskname: {1}", t.Id, t.Name, t.CreationDate);
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
            Console.WriteLine("Shutting down...");
        }
    }
}
