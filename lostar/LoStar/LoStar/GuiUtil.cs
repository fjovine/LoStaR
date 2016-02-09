//-----------------------------------------------------------------------
// <copyright file="GuiUtil.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;

    /// <summary>
    /// This class contains standard static methods used for GUI purposes.
    /// </summary>
    public static class GuiUtil
    {
        /// <summary>
        /// Saves the current position and size of the MainWindow in the registry.
        /// </summary>
        /// <param name="x">Left coordinate of the window in pixels.</param>
        /// <param name="y">Top coordinate of the window in pixels.</param>
        /// <param name="w">Width of the window in pixels.</param>
        /// <param name="h">Height of the window in pixels.</param>
        public static void SaveMainWindowPositionInRegistry(double x, double y, double w, double h)
        {
            WindowsRegistry.Set(
                WindowsRegistryEntry.STOREDWINDOWSPOSITION,
                new List<string>() { ((int)x).ToString(), ((int)y).ToString(), ((int)w).ToString(), ((int)h).ToString() });
        }

        /// <summary>
        /// Retrieves the position and size of main window of the App from the registry.
        /// Every time the main window is moved or resized, its new position is stored into the registry to reopen it in
        /// the same position when the app is successively launched.
        /// </summary>
        /// <returns>The rectangle on the computer screen occupied by the app window.</returns>
        public static Rect LoadMainWindowPositionFromRegistry()
        {
            Rect result = new Rect(20, 20, 1400, 600);
            List<string> position = WindowsRegistry.Gets(WindowsRegistryEntry.STOREDWINDOWSPOSITION);
            for (int i = 0; i < position.Count; i++)
            {
                double val = double.Parse(position[i]);
                switch (i)
                {
                    case 0: result.X = val;
                        break;
                    case 1: result.Y = val;
                        break;
                    case 2: result.Width = val;
                        break;
                    case 3: result.Height = val;
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the object of type T in the visual sub tree starting from the passed root.
        /// </summary>
        /// <typeparam name="T">Class of the desired object.</typeparam>
        /// <param name="visual">Root of the visual hierarchy to start the search.</param>
        /// <returns>The object of the required type if found, null otherwise.</returns>
        public static T GetVisualChild<T>(DependencyObject visual)
            where T : DependencyObject
        {
            if (visual == null)
            {
                return null;
            }

            var count = VisualTreeHelper.GetChildrenCount(visual);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(visual, i);

                var childOfTypeT = child as T ?? GetVisualChild<T>(child);
                if (childOfTypeT != null)
                {
                    return childOfTypeT;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the cell in a row. If the cell is virtualized, i.e. the row is outside the visible screen,
        /// then it is created.
        /// For deeper details see <c>http://social.technet.microsoft.com/wiki/contents/articles/21202.wpf-programmatically-selecting-and-focusing-a-row-or-cell-in-a-datagrid.aspx</c>
        /// </summary>
        /// <param name="dataGrid">Data grid where the cell is to be found.</param>
        /// <param name="rowContainer">Row container where the cell is to be found.</param>
        /// <param name="column">Index of the column where the cell is located in the row.</param>
        /// <returns>The selected cell in the data grid.</returns>
        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    // if the row has been virtualized away, call its ApplyTemplate() method
                    // to build its visual tree in order for the DataGridCellsPresenter
                    // and the DataGridCells to be created
                    rowContainer.ApplyTemplate();
                    presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                }

                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        // bring the column into view in case it has been virtualized away
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }

                    return cell;
                }
            }

            return null;
        }

        /// <summary>
        /// Extension method that selects the row of a <c>DataGrid</c> from its row index.
        /// </summary>
        /// <param name="dataGrid"><c>DataGrid</c> the extension methods refers to.</param>
        /// <param name="rowIndex">Index of the row to retrieve.</param>
        public static void SelectRowByIndex(this DataGrid dataGrid, int rowIndex)
        {
            if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.FullRow))
            {
                throw new ArgumentException("The SelectionUnit of the DataGrid must be set to FullRow.");
            }

            // If the list is empty, there is nothing to select
            if (dataGrid.Items.Count == 0)
            {
                return;
            }

            if (rowIndex < 0 || rowIndex > (dataGrid.Items.Count - 1))
            {
                throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));
            }

            /* set the SelectedItem property */
            object item = dataGrid.Items[rowIndex]; // = Product X
            dataGrid.SelectedItem = item;

            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            if (row == null)
            {
                // bring the data item (Product object) into view in case it has been virtualized away
                dataGrid.ScrollIntoView(item);
                row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            }

            if (row != null)
            {
                DataGridCell cell = GetCell(dataGrid, row, 0);
                if (cell != null)
                {
                    cell.Focus();
                }
            }
        }
    }
}
