using Autofac;

namespace ch.jaxx.TaskManager.DataAccess
{
    public class TaskManagerConfiguratioModule : Module
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
