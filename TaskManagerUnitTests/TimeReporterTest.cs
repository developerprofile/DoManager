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
    }
}
