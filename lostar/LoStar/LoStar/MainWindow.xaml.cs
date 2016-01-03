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
    /// Delegate used to manage the cursor movement.
    /// </summary>
    /// <param name="cursorPosition">New position of the cursor in seconds.</param>
    public delegate void CursorHandler(double cursorPosition);

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow" /> class.
    /// </summary>
    public partial class MainWindow : Window, ITimelineSegment
    {
        /// <summary>
        /// Local repository of the cursor time.
        /// </summary>
        private double cursorTime;

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
        /// Event triggered by a cursor movement.
        /// </summary>
        public event CursorHandler OnCursorChange;

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
        /// Gets the duration in second of the window.
        /// </summary>
        public double WindowDuration
        {
            get
            {
                return this.MaxShownTime - this.MinShownTime;
            }
        }

        /// <summary>
        /// Gets or sets the time position of the cursor on screen in seconds.
        /// </summary>
        public double CursorTime
        {
            get
            {
                return this.cursorTime;
            }

            set 
            {
                this.cursorTime = value;
                if (this.OnCursorChange != null)
                {
                    this.OnCursorChange(value);
                }
            }
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
        public void PerformZoom(double factor)
        {
            double deltaBefore = this.CursorTime - this.MinShownTime;
            double deltaAfter = this.MaxShownTime - this.CursorTime;

            this.MinShownTime += deltaBefore * factor;
            this.MaxShownTime -= deltaAfter * factor;
            this.OnZoom();
        }

        /// <summary>
        /// Scrolls the current window moving to the right or to the left the current
        /// window.
        /// The factor determines how the scroll is performed: a negative value scrolls leftwards (towards
        /// the past) a positive value scrolls rightwards (towards the future).
        /// </summary>
        /// <param name="factor">Determines the scrolling percentage. It ranges from -1 to 1. 
        /// -1 means that the window is scrolled completely to the left
        /// +1 means that the window is scrolled completely to the right</param>
        public void Scroll(double factor)
        {
            double scrollSize = this.WindowDuration * factor;
            this.MinShownTime += scrollSize;
            this.MaxShownTime += scrollSize;
            this.OnZoom();
        }
    }
}