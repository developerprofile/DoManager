using ch.jaxx.TaskManager.DataAccess;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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

        public TimeLogViewModel(TaskManager TaskManager)
        {
            taskMan = TaskManager;
            SelectedDate = DateTime.Now - new TimeSpan(1, 0, 0, 0);
            OnExportTimeLog = new RelayCommand(ExecuteExportTimeLog);
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
                RaisePropertyChanged();
            }
        }


        public ICommand OnExportTimeLog { get; private set; }


        private void ExecuteExportTimeLog()
        {
            taskMan.LogTaskDurations(SelectedDate);
        }
    }
}
