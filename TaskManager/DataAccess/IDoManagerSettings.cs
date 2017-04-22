using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ch.jaxx.TaskManager.DataAccess
{
    public interface IDoManagerSettings
    {
        string TimeReportExportPath { get; set; }        
    }
}
