using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using ch.jaxx.TaskManager.DataAccess;
using Ploeh.AutoFixture;

namespace TaskManagerUnitTests
{
    [TestFixture]
    public class TaskPhaseTimeReportTest
    {

        private static IContainer Container { get; set; }


        private List<ITaskTaskPhaseConnector> GenerateTestData()
        {
            return new List<ITaskTaskPhaseConnector>()
            {
                GetTaskConnetor(1), GetTaskConnetor (2)
            };             
        }

        private ITaskTaskPhaseConnector GetTaskConnetor(int Id)
        {
            ITaskTaskPhaseConnector connector;            
            using (var scope = Container.BeginLifetimeScope())
            {
                connector = scope.Resolve<ITaskTaskPhaseConnector>();
            }

            connector.OwnerTask = GetTestTask(Id);
            connector.MemberTaskPhases = GetTestTaskPhases(Id);

            return connector;            
        }

        private ITask GetTestTask(int Id)
        {
            ITask task;
            using (var scope = Container.BeginLifetimeScope())
            {
                task = scope.Resolve<ITask>();                    ;
            }
            
            switch (Id)
            {
                case 1:
                    task.Id = 1;
                    task.Name = "Task1";
                    task.StartDate = new DateTime(2015, 10, 1, 12, 00, 00);
                    task.DoneDate = new DateTime(2015, 10, 1, 13, 00, 00);
                    task.State = TaskState.DONE;
                    break;
                case 2:
                    task.Id = 2;
                    task.Name = "Task2";
                    task.StartDate = new DateTime(2015, 10, 2, 12, 00, 00);
                    task.DoneDate = new DateTime(2015, 10, 2, 13, 00, 00);
                    task.State = TaskState.DONE;
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            return task;
        }

        private List<ITaskPhase> GetTestTaskPhases(int TaskId)
        {
            ITaskPhase taskPhase;
            List<ITaskPhase> taskPhaseList = new List<ITaskPhase>();
            using (var scope = Container.BeginLifetimeScope())
            {
                taskPhase = scope.Resolve<ITaskPhase>(); ;
            }

            taskPhase.Id = 1;
            taskPhase.TaskId = TaskId;
            taskPhase.StartDate = new DateTime(2015, 10, 1, 12, 00, 00);
            taskPhase.EndDate = new DateTime(2015, 10, 1, 13, 00, 00);

            taskPhaseList.Add(taskPhase);

            taskPhase.Id = 2;
            taskPhase.TaskId = TaskId;
            taskPhase.StartDate = new DateTime(2015, 10, 1, 14, 00, 00);
            taskPhase.EndDate = new DateTime(2015, 10, 1, 15, 00, 00);

            taskPhaseList.Add(taskPhase);

            return taskPhaseList;

        }

        
        [TestFixtureSetUp]
        public void SetUpTest()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new IoCTestModule());
            Container = builder.Build();
        }

        [TestCase]
        public void A01_GetReportTest()
        {
            var testData = GenerateTestData();

            var expected = new List<string>()
            {
                "Task: Task1, PhasesDuration: 02:00:00",
                "Task: Task2, PhasesDuration: 02:00:00"
            };

            ITimeReporter reporter;
            using (var scope = Container.BeginLifetimeScope())
            {
                reporter = scope.Resolve<ITimeReporter>();
            }
            ITimeReport report = new TaskPhaseTimeReport(reporter);
            var actual = report.ReportList(testData,null,null);
            Assert.AreEqual(expected, actual);
        }
    }
}
