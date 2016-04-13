using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace DoManagerMui.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private string selectedDatabaseFile;

        public SettingsViewModel()
        {
            OnChooseDatabase = new RelayCommand<EventArgs>(ExecuteChooseDatabase);

        }

        public string SelectedDatabaseFile
        {
            get
            {
                return selectedDatabaseFile;
            }
            set
            {
                if (selectedDatabaseFile == value)
                    return;
                selectedDatabaseFile = value;
                RaisePropertyChanged();
            }
        }

        public ICommand OnChooseDatabase
        {
            get;
            private set;
        }

        private void ExecuteChooseDatabase(EventArgs obj)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog().Value)
            {
                SelectedDatabaseFile = dialog.FileName;
            }
        }
    }
}
