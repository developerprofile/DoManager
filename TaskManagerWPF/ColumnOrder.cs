using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TaskManagerWPF
{
    /// <summary>
    /// Static class for handle column order and width of the WPF datagrid
    /// </summary>
    internal static class ColumnOrder
    {

        /// <summary>
        /// To save the column order within session we use this variable
        /// </summary>
        private static List<ColumnOrderItem> columnOrder;

        /// <summary>
        /// Saves the column order of this session
        /// </summary>
        /// <param name="taskGrid"></param>
        internal static void SaveColumnOrder(DataGrid taskGrid)
        {
            if (taskGrid.CanUserReorderColumns)
            {
                columnOrder = new List<ColumnOrderItem>();
                columnOrder.Clear();
                var columns = taskGrid.Columns;
                for (int i = 0; i < columns.Count; i++)
                {
                    columnOrder.Add(new ColumnOrderItem
                    {
                        ColumnIndex = i,
                        DisplayIndex = columns[i].DisplayIndex,
                        Width = columns[i].Width
                    });
                }
            }
        }


        /// <summary>
        /// Sets the save colum order of this WPF session
        /// </summary>
        /// <param name="taskGrid"></param>
        internal static void SetColumnOrder(DataGrid taskGrid)
        {

            if (columnOrder != null)
            {
                var sorted = columnOrder.OrderBy(i => i.DisplayIndex);
                foreach (var item in sorted)
                {
                    taskGrid.Columns[item.ColumnIndex].DisplayIndex =
                                        item.DisplayIndex;
                    
                    taskGrid.Columns[item.ColumnIndex].Width = item.Width;
                }
            }
        }
    }
}
