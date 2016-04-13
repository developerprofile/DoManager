using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;

namespace DoManagerMui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            Dapplo.LogFacade.Loggers.NLogLogger.RegisterLogger(Dapplo.LogFacade.LogLevel.Verbose);
            InitializeComponent();
        }

        internal void SetTaskBarIconActive()
        {
            this.TaskbarItemInfo.Overlay = (ImageSource)Resources["OverlayImage"];
            this.TaskbarItemInfo.ProgressValue = 100;
            this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
        }

        internal void SetTaskBarIconPaused()
        {
            this.TaskbarItemInfo.Overlay = null;
            this.TaskbarItemInfo.ProgressValue = 100;
            this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
        }
    }
}
