using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TaskManagerWPF
{
    /// <summary>
    /// Class for persist a datagrids column order and width
    /// </summary>
    [Serializable]
    public sealed class ColumnOrderItem
    {
        public int DisplayIndex { get; set; }
        public DataGridLength Width { get; set; }
        public bool Visible { get; set; }
        public int ColumnIndex { get; set; }
    }
}
