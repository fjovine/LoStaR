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
        /// Duration of the window when zoomed to the maximum scale.
        /// </summary>
        private static readonly double MaxZoomSizeSeconds = 1000.0E-6;

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
            this.TimelineSegment.PerformZoom(0.5);
        }

        /// <summary>
        /// Zoom the window out (less detail) keeping the cursor in the same position on screen.
        /// The new scale factor is doubled.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            this.TimelineSegment.PerformZoom(-2);
        }

        /// <summary>
        /// Zooms to the maximum available zoom.
        /// The maximum zoom is chosen so that the whole window will show MaxZoomSizeSeconds microseconds.
        /// The cursor will be left where it was.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ZoomInMax_Click(object sender, RoutedEventArgs e)
        {
            double deltaBefore = this.TimelineSegment.CursorTime - this.TimelineSegment.MinShownTime;
            double deltaAfter = this.TimelineSegment.MaxShownTime - this.TimelineSegment.CursorTime;

            this.TimelineSegment.MinShownTime = this.TimelineSegment.CursorTime - ((deltaBefore * MaxZoomSizeSeconds) / (deltaAfter + deltaBefore));
            this.TimelineSegment.MaxShownTime = this.TimelineSegment.MinShownTime + MaxZoomSizeSeconds;
            this.TimelineSegment.PerformZoom(0);
        }

        /// <summary>
        /// Scrolls the window leftwards half of its duration.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void LeftDArrow_Click(object sender, RoutedEventArgs e)
        {
            this.TimelineSegment.Scroll(-0.5);
        }

        /// <summary>
        /// Scrolls the window rightwards half of its duration.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void RightDArrow_Click(object sender, RoutedEventArgs e)
        {
            this.TimelineSegment.Scroll(0.5);
        }

        /// <summary>
        /// Zooms the window to contain all the available time period.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ZoomAll_Click(object sender, RoutedEventArgs e)
        {
            this.TimelineSegment.ZoomAll();
        }

        /// <summary>
        /// Moves the cursor 1/40 of the windows duration leftwards.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void LeftArrow_Click(object sender, RoutedEventArgs e)
        {
            this.TimelineSegment.CursorTime -= this.TimelineSegment.WindowDuration / 40;
            this.TimelineSegment.PerformZoom(0);
        }

        /// <summary>
        /// Moves the cursor 1/40 of the windows duration rightwards.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void RightArrow_Click(object sender, RoutedEventArgs e)
        {
            this.TimelineSegment.CursorTime += this.TimelineSegment.WindowDuration / 40;
            this.TimelineSegment.PerformZoom(0);
        }

        /// <summary>
        /// Scrolls the window in order to place the cursor in the center of the window.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void CenterCursor_Click(object sender, RoutedEventArgs e)
        {
            this.TimelineSegment.CenterCursor();
        }

        /// <summary>
        /// Moves the cursor to the following event in the selects stripe.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void LeftSignificance_Click(object sender, RoutedEventArgs e)
        {
            IBrowsableTimeline currentBrowsableTimeline = this.TimelineSegment.BrowsableTimeline;
            if (currentBrowsableTimeline != null)
            {
                double? priorEvent = currentBrowsableTimeline.GetNearestEventBefore(this.TimelineSegment.CursorTime);
                if (priorEvent != null)
                {
                    this.TimelineSegment.CursorTime = (double)priorEvent;
                    if (this.TimelineSegment.CursorTime < this.TimelineSegment.MinShownTime)
                    {
                        this.TimelineSegment.CenterCursor();
                    }
                    else
                    {
                        this.TimelineSegment.PerformZoom(0);
                    }
                }
            }
        }

        /// <summary>
        /// Moves the cursor to the previous event in the selects stripe.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void RightSignificance_Click(object sender, RoutedEventArgs e)
        {
            IBrowsableTimeline currentBrowsableTimeline = this.TimelineSegment.BrowsableTimeline;
            if (currentBrowsableTimeline != null)
            {
                double? laterEvent = currentBrowsableTimeline.GetNearestEventAfter(this.TimelineSegment.CursorTime);
                if (laterEvent != null)
                {
                    this.TimelineSegment.CursorTime = (double)laterEvent;
                    if (this.TimelineSegment.CursorTime > this.TimelineSegment.MaxShownTime)
                    {
                        this.TimelineSegment.CenterCursor();
                    }
                    else
                    {
                        this.TimelineSegment.PerformZoom(0);
                    }
                }
            }
        }
    }
}
