using ch.jaxx.TaskManager.DataAccess;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DoManagerMui.ViewModel
{
    public class TimeLogViewModel : ViewModelBase
    {
        private TaskManager taskMan;
        private DateTime selectedDate;
        private IEnumerable<string> phaseList;

        public TimeLogViewModel(TaskManager TaskManager)
        {
            taskMan = TaskManager;
            SelectedDate = DateTime.Now - new TimeSpan(1, 0, 0, 0);
            OnExportTimeLog = new RelayCommand(ExecuteExportTimeLog);
            GotMouseCapture = new RelayCommand<MouseEventArgs>(ExecuteGotMouseCapture);

            Messenger.Default.Register<NavigationMessage>(this, p =>
            {
                if (p.IsRefreshRequested)
                {
                    RefreshPhaseList(SelectedDate);
                }
            });
        }

        public DateTime SelectedDate
        {
            get
            {
                return selectedDate;
            }
            set
            {
                if (selectedDate == value)
                    return;
                selectedDate = value;
                RefreshPhaseList(value);
                RaisePropertyChanged();
            }
        }

        public IEnumerable<string> PhaseList
        {
            get
            {
                return phaseList;
            }
            set
            {
                if (phaseList == value)
                    return;
                phaseList = value;
                RaisePropertyChanged();
            }
        }


        public ICommand OnExportTimeLog { get; private set; }
        public ICommand GotMouseCapture { get; private set; }


        private void ExecuteExportTimeLog()
        {
            taskMan.LogTaskDurations(SelectedDate);
        }

        private void ExecuteGotMouseCapture(MouseEventArgs obj)
        {
            if (Mouse.Captured is System.Windows.Controls.Calendar || Mouse.Captured is System.Windows.Controls.Primitives.CalendarItem)
            {
                // free mouse capture from calendar
                Mouse.Capture(null);
            }
        }

        private void RefreshPhaseList(DateTime Day)
        {
            PhaseList = taskMan.GetTaskPhaseReport(Day);
        }

    }
}
