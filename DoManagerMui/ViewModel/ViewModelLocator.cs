/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:DoManagerMui"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using Autofac;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using ch.jaxx.TaskManager.DataAccess;
using System.IO;

namespace DoManagerMui.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private IContainer viewContainer;

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            string databaseFileName;
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // set a path to a copy of TEMPLATE.FDB in order to get a XAML design preview
                databaseFileName = @"D:\TEMP\WORK3.FDB";
            }
            else
            {
                databaseFileName = GetDatabase();
                if (databaseFileName == "") Environment.Exit(-1);
            }
            var connectionString = String.Format(@"server type=Embedded;user id=sysdba;password=masterky;dialect=3;character set=UTF8;client library=fbembed.dll;database={0}", databaseFileName);

            var settings = new DoManagerSettings
            {
                TimeReportExportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DoManager", "Reports")
            };

            var taskMan = new TaskManager(connectionString, settings);

            var builder = new ContainerBuilder();
            builder.RegisterType<MainViewModel>();
            builder.RegisterType<TaskListViewModel>().WithParameters(new TypedParameter[]
                {
                    new TypedParameter(typeof(TaskManager), taskMan),
                    new TypedParameter(typeof(MainWindow), App.Current.MainWindow),
                    new TypedParameter(typeof(string),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DoManager", "TodoTxt","todo.txt"))
                });
            builder.RegisterType<TimeLogViewModel>().WithParameters(new TypedParameter[]
                {
                    new TypedParameter(typeof(TaskManager), taskMan),
                    new TypedParameter(typeof(IDoManagerSettings), settings)
                });
            builder.RegisterType<SettingsViewModel>();
            viewContainer = builder.Build();
        }

        private string GetDatabase()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Firebird Database (*.fdb)|*.fdb|All files (*.*)|*.*";
            dialog.Title = "Choose a database";
            if (dialog.ShowDialog().Value)
            {
                return (dialog.FileName);
            }
            else return "";
        }

        public MainViewModel Main
        {
            get
            {
                using (var scope = viewContainer.BeginLifetimeScope())
                {
                    return scope.Resolve<MainViewModel>();
                }
            }
        }

        public SettingsViewModel SettingsView
        {
            get
            {
                using (var scope = viewContainer.BeginLifetimeScope())
                {
                    return scope.Resolve<SettingsViewModel>();
                }
            }
        }

        public TaskListViewModel TaskListView
        {
            get
            {
                using (var scope = viewContainer.BeginLifetimeScope())
                {
                    return scope.Resolve<TaskListViewModel>();
                }
            }
        }

        public TimeLogViewModel TimeLogView
        {
            get
            {
                using (var scope = viewContainer.BeginLifetimeScope())
                {
                    return scope.Resolve<TimeLogViewModel>();
                }
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}