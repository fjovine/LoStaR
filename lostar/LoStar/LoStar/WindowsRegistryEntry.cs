//-----------------------------------------------------------------------
// <copyright file="WindowsRegistryEntry.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    /// <summary>
    /// This class contains a list of strings for the registry entries.
    /// So registry string should be disseminated in the code: only strings coming
    /// from this class should be used.
    /// </summary>
    public static class WindowsRegistryEntry
    {
        /// <summary>
        /// Registry entry containing an array of strings representing the most recent files opened.
        /// </summary>
        public const string RECENTFILES = "RecentFiles";

        /// <summary>
        /// Registry entry containing the name of the last folder opened.
        /// </summary>
        public const string THELASTFOLDER = "TheLastFolder";

        /// <summary>
        /// Registry entry containing position and size of the main window.
        /// </summary>
        public const string STOREDWINDOWSPOSITION = "StoredWindowsPosition";
    }
}
