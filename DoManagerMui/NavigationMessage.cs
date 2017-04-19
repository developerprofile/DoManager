using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoManagerMui
{
    /// <summary>
    /// Provides messages which could passed from one MUI View to another
    /// </summary>
    public class NavigationMessage
    {
        /// <summary>
        /// If true, a refresh for the view is requested
        /// </summary>
        public bool IsRefreshRequested { get; set; }
    }
}
