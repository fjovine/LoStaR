//-----------------------------------------------------------------------
// <copyright file="ToolBar.xaml.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Toolbar containing all the commands to be executed by the stripe container.
    /// </summary>
    public partial class ToolBar : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolBar" /> class.
        /// </summary>
        public ToolBar()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a reference to the TimelineSegment interface that manipulates the GUI.
        /// </summary>
        public ITimelineSegment TimelineSegment
        {
            get;
            set;
        }

        /// <summary>
        /// Zoom the window in (more detail) keeping the cursor in the same position on screen.
        /// The new scale factor is halved.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            this.TimelineSegment.ZoomIn();
        }

        /// <summary>
        /// Zoom the window out (less detail) keeping the cursor in the same position on screen.
        /// The new scale factor is doubled.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            this.TimelineSegment.ZoomOut();
        }

        /// <summary>
        /// Zooms to the maximum available zoom, i.e. making visible all the available data.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ZoomInMax_Click(object sender, RoutedEventArgs e)
        {
            this.TimelineSegment.ZoomAll();
        }
    }
}
