using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch.jaxx.TaskManager.DataAccess
{
    public class DoManagerSettings : IDoManagerSettings
    {
        private string _timeReportExportPath;

        public string TimeReportExportPath
        {
            get
            {
                if (_timeReportExportPath != null)
                {
                    return _timeReportExportPath;
                }
                else throw new ArgumentNullException("TimeReportExportPath");
            }
            set
            {
                _timeReportExportPath = value;
            }
        }
    }
}
