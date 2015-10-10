using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ch.jaxx.TaskManager.DataAccess;
using Ploeh.AutoFixture;

namespace TaskManagerUnitTests
{
    [TestFixture]
    class TimeReporterTest
    {

        [TestCase]        
        public void A01_GetTaskDurationTest()
        {
            Fixture fixture = new Fixture();

            var startDate = new DateTime(2015, 10, 1, 12, 00, 00);
            var endDate = new DateTime(2015, 10, 3, 13, 00, 00);
            var expectedDuration = new TimeSpan(2, 1, 0, 0);

            ITask testTask = new TaskModel
            {
                Id = 1,
                Name = fixture.Create<string>(),
                CreationDate = fixture.Create<DateTime>(),
                State = TaskState.DONE,
                StartDate = startDate,
                DoneDate = endDate
            };

            ITimeReporter reporter = new TimeReporter();
            var actualDuration = reporter.GetTaskDuration(testTask);

            Assert.AreEqual(expectedDuration, actualDuration);
            
        }

        [TestCase]
        public void A02_GetSumTaskDurationTest()
        {
            
            ITaskPhase phaseOne = new TaskPhaseModel()
            {
                Id = 1,
                TaskId = 1,
                StartDate = new DateTime(2015, 10, 1, 00, 00, 00),
                EndDate = new DateTime(2015, 10, 1, 01, 00, 00)
            };

            ITaskPhase phaseTwo = new TaskPhaseModel()
            {
                Id = 2,
                TaskId = 1,
                StartDate = new DateTime(2015, 10, 1, 03, 00, 00),
                EndDate = new DateTime(2015, 10, 1, 03, 30, 00)
            };

            ITaskPhase phaseThree = new TaskPhaseModel()
            {
                Id = 3,
                TaskId = 1,
                StartDate = new DateTime(2015, 10, 1, 05, 00, 00),
                EndDate = new DateTime(2015, 10, 1, 05, 10, 20)
            };

            var taskPhaseList = new List<ITaskPhase>();
            taskPhaseList.Add(phaseOne);
            taskPhaseList.Add(phaseTwo);
            taskPhaseList.Add(phaseThree);

            var expectedDuration = new TimeSpan(1, 40, 20);

            ITimeReporter reporter = new TimeReporter();
            var actualDuration = reporter.GetTaskPhasesDuration(taskPhaseList);

            Assert.AreEqual(expectedDuration, actualDuration);
        }
    }
}
