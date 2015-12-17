//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
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

            this.MinShownTime = 1.35;
            this.MaxShownTime = 2.1;

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

            bool state = false;

            this.MouseDoubleClick += (o, a) =>
                {
                    if (state)
                    {
                        this.MinShownTime = 0;
                        this.MaxShownTime = 10;
                        this.CursorCanvas.CursorPosition = 5;
                        this.CursorCanvas.ClearAuxiliaryCursors();
                        for (double x = 5.1; x < 6; x += 0.1)
                        {
                            this.CursorCanvas.AddAuxiliaryCursor(x, Brushes.Green);
                        }
                    }
                    else
                    {
                        this.MinShownTime = 1.35;
                        this.MaxShownTime = 2.1;
                        this.CursorCanvas.CursorPosition = 1.6;
                        this.CursorCanvas.ClearAuxiliaryCursors();
                    }

                    state = !state;
                    if (this.OnZoom != null)
                    {
                        OnZoom();
                    }
                };
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
    }
}