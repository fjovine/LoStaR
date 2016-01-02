//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Delegate used to manage the Zoom event.
    /// </summary>
    public delegate void ZoomHandler();

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow" /> class.
    /// </summary>
    public partial class MainWindow : Window, ITimelineSegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.ToolBar.TimelineSegment = this;

            this.StripeContainer.SizeChanged += (s, a) =>
                {
                    if (this.OnZoom != null)
                    {
                        OnZoom();
                    }
                };

            this.MinTime = 0;
            this.MinShownTime = 1.35;
            this.MaxShownTime = 2.1;
            this.MaxTime = 5;

            DigitalTimeline timeline = new DigitalTimeline();
            timeline.InitialState = false;
            for (double transition = 0; transition < 10; transition += 1)
            {
                timeline.Transitions.Add(transition);
            }

            this.TimeAxis.Timeline = timeline;
            this.TimeAxis.TimelineSegment = this;

            this.Stripe0.Caption = "Stripe 0";
            this.Stripe0.Timeline = timeline;
            this.Stripe0.TimelineSegment = this;

            timeline = new DigitalTimeline();
            for (double transition = 0; transition < 10; transition += 0.5)
            {
                timeline.Transitions.Add(transition);
            }

            this.Stripe1.Caption = "Stripe 1";
            this.Stripe1.IsSelected = true;
            this.Stripe1.Timeline = timeline;
            this.Stripe1.TimelineSegment = this;

            this.CursorCanvas.Timeline = timeline;
            this.CursorCanvas.TimelineSegment = this;
        }

        /// <summary>
        /// Event triggered following a zoom request.
        /// </summary>
        public event ZoomHandler OnZoom;

        /// <summary>
        /// Gets or sets the minimum time in seconds for which data is available.
        /// </summary>
        public double MinTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum time in seconds for which data is available.
        /// </summary>
        public double MaxTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum time shown on the timeline.
        /// </summary>
        public double MinShownTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum time shown on the timeline.
        /// </summary>
        public double MaxShownTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time position of the cursor on screen in seconds.
        /// </summary>
        public double CursorTime
        {
            get;
            set;
        }

        /// <summary>
        /// Zooms in the timeline, i.e. shows a more detailed view.
        /// </summary>
        public void ZoomIn()
        {
            this.PerformZoom(0.5);
        }

        /// <summary>
        /// Zooms out the timeline, i.e. shows a less detailed view.
        /// </summary>
        public void ZoomOut()
        {
            this.PerformZoom(-2);
        }

        /// <summary>
        /// Shows all the timeline on the available data.
        /// </summary>
        public void ZoomAll()
        {
            this.MinShownTime = this.MinTime;
            this.MaxShownTime = this.MaxTime;
            this.OnZoom();
        }

        /// <summary>
        /// Computes the new extremes of the window so that the cursor stays where it is and the scale factor is changed according to the passed parameter.
        /// </summary>
        /// <param name="factor">Zoom factor used to compute the new extremes of the window.</param>
        private void PerformZoom(double factor)
        {
            double deltaBefore = this.CursorTime - this.MinShownTime;
            double deltaAfter = this.MaxShownTime - this.CursorTime;

            this.MinShownTime += deltaBefore * factor;
            this.MaxShownTime -= deltaAfter * factor;
            this.OnZoom();
        }
    }
}