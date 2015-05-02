using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch.jaxx.TaskManager.DataAccess
{
    public class TestAccess
    {
        public void GetTasks()
        {
            using (var ctx = new FirebirdContext(@"server type=Embedded;user id=sysdba;password=masterky;dialect=3;character set=UTF8;client library=fbembed.dll;database=D:\task.fdb"))
            {
                //var task = new TaskModel() { Name = DateTime.Now.Ticks.ToString() };
                //ctx.Tasks.Add(task);
                //ctx.SaveChanges();

                foreach (var t in ctx.Tasks)
                {
                    Console.WriteLine("TaskId: {0}; Taskname: {1}", t.Id, t.Name);
                }
            }

            Console.ReadKey();
        }
    }
}
