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
    }
}
