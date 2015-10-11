using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ch.jaxx.TaskManager.DataAccess;

namespace TaskManagerUnitTests
{
    internal class IoCTestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(t => new TaskModel()).As<ITask>();
            builder.Register(t => new TaskPhaseModel()).As<ITaskPhase>();
            builder.Register(t => new TaskTaskPhaseConnector()).As<ITaskTaskPhaseConnector>();
            builder.Register(t => new TimeReporter()).As<ITimeReporter>();
            builder.Register(t => new TaskPhaseTimeReport(t.Resolve<ITimeReporter>())).As<ITimeReport>();
        }
    }
}
