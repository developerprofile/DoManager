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
        private List<ITaskPhase> testTaskPhaseSetOne
        {
            get
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
                    StartDate = new DateTime(2015, 10, 2, 03, 00, 00),
                    EndDate = new DateTime(2015, 10, 2, 03, 30, 00)
                };

                ITaskPhase phaseThree = new TaskPhaseModel()
                {
                    Id = 3,
                    TaskId = 1,
                    StartDate = new DateTime(2015, 10, 3, 05, 00, 00),
                    EndDate = new DateTime(2015, 10, 3, 05, 10, 20)
                };

                ITaskPhase phaseFour = new TaskPhaseModel()
                {
                    Id = 4,
                    TaskId = 1,
                    StartDate = new DateTime(2015, 10, 10, 23, 55, 00),
                    EndDate = new DateTime(2015, 10, 11, 00, 05, 00)
                };

                ITaskPhase phaseFive = new TaskPhaseModel()
                {
                    Id = 5,
                    TaskId = 1,
                    StartDate = new DateTime(2015, 10, 13, 23, 00, 00),
                    EndDate = new DateTime(2015, 10, 15, 00, 00, 00)
                };

                var taskPhaseList = new List<ITaskPhase>();
                taskPhaseList.Add(phaseOne);
                taskPhaseList.Add(phaseTwo);
                taskPhaseList.Add(phaseThree);
                taskPhaseList.Add(phaseFour);
                taskPhaseList.Add(phaseFive);

                return taskPhaseList;
            }
        }           

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
            var testTaskPhaseSet = testTaskPhaseSetOne;
            var expectedDuration = new TimeSpan(1,2, 50, 20);

            ITimeReporter reporter = new TimeReporter();
            var actualDuration = reporter.GetTaskPhasesDuration(testTaskPhaseSet);

            Assert.AreEqual(expectedDuration, actualDuration);
        }


        [TestCase]
        public void A03_GetSumTaskDurationByDateRange()
        {
            var testTaskPhaseSet = testTaskPhaseSetOne;
            var expectedDuration = new TimeSpan(0, 50, 20);

            var reporter = new TimeReporter();
            var actualDuration = reporter.GetTaskPhasesDuration(testTaskPhaseSet, new DateTime(2015, 10, 02), new DateTime(2015, 10, 14));

            Assert.AreEqual(expectedDuration, actualDuration);
        }

        [TestCase]
        public void A04_GetSumTaskDurationByOneDateOnly()
        {
            var testTaskPhaseSet = testTaskPhaseSetOne;
            var expectedDuration = new TimeSpan(0, 10, 20);

            var reporter = new TimeReporter();
            var actualDuration = reporter.GetTaskPhasesDuration(testTaskPhaseSet, new DateTime(2015, 10, 03), new DateTime(2015, 10, 04));

            Assert.AreEqual(expectedDuration, actualDuration);
        }

        [TestCase]
        public void A05_GetSumTaskDurationByFateRangePassingNull()
        {
            var testTaskPhaseSet = testTaskPhaseSetOne;
            var expectedDuration = new TimeSpan(1, 2, 50, 20);

            var reporter = new TimeReporter();

            // Pass null instead of fromDate > sum without filter
            var actualDuration = reporter.GetTaskPhasesDuration(testTaskPhaseSet, null, new DateTime(2015, 10, 04));
            Assert.AreEqual(expectedDuration, actualDuration);
            // Pass null instead of endDate > sum without filter
            actualDuration = reporter.GetTaskPhasesDuration(testTaskPhaseSet, new DateTime(2015, 10, 03), null);
            Assert.AreEqual(expectedDuration, actualDuration);
            // Pass null instead of fromDate and endDate > sum wihtout filter
            actualDuration = reporter.GetTaskPhasesDuration(testTaskPhaseSet, null, null);
            Assert.AreEqual(expectedDuration, actualDuration);
        }
    }
}
