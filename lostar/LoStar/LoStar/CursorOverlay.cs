//-----------------------------------------------------------------------
// <copyright file="CursorOverlay.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Overlay canvas that is used to draw the main cursor and the auxiliary cursors.
    /// The main cursor is normally modifiable directly with the cursor.
    /// The auxiliary cursor are normally less dynamic than the main cursor
    /// and show some computed information on the stripes.
    /// On top of this canvas, there is a stripe that represents the position of the current window in the whole string of available data.
    /// An "absolute cursor" is shown in this widget, representing the position of the cursor in absolute coordinates, 
    /// i.e. with respect to the whole period of time available.
    /// </summary>
    public class CursorOverlay : Stripe
    {
        /// <summary>
        /// Stat of the top cursor expressed as a percentage of the top stripe.
        /// </summary>
        private const double StartTopCursor = 0.4;

        /// <summary>
        /// Height of the top cursor expressed as a percentage of the top stripe.
        /// </summary>
        private const double HeightTopCursor = 0.4;

        /// <summary>
        /// Backup field of the CursorPosition property.
        /// </summary>
        private double cursorPosition;

        /// <summary>
        /// Private copy of the enumeration of managed stripes
        /// </summary>
        private IEnumerable<SelectableStripe> managedStripes;

        /// <summary>
        /// Private copy of the container of the selectable stripes managed by the cursor overlay
        /// </summary>
        private UIElement selectableStripesContaner;

        /// <summary>
        /// List of the auxiliary cursors.
        /// </summary>
        private List<AuxiliaryCursor> auxiliaryCursors = new List<AuxiliaryCursor>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CursorOverlay" /> class.
        /// </summary>
        public CursorOverlay()
        {
            this.MouseDown += (s, a) =>
                {
                    if (a.ClickCount == 1)
                    {
                        double cursorTime = this.DescaleX(a.GetPosition(this).X);
                        if (a.MiddleButton == MouseButtonState.Pressed)
                        {
                            this.TimelineSegment.AnchorTime = cursorTime;
                        }

                        this.TimelineSegment.CursorTime = cursorTime;
                        a.Handled = true;
                    }
                    else
                    {
                        SelectableStripe hitStripe = null;

                        Point pt = a.GetPosition(this.selectableStripesContaner);
                        VisualTreeHelper.HitTest(
                            this.selectableStripesContaner,
                            null,
                            (r) => 
                            {
                                if (r.VisualHit is SelectableStripe)
                                {
                                    hitStripe = (SelectableStripe)r.VisualHit;
                                    return HitTestResultBehavior.Stop;
                                }
                                else
                                {
                                    return HitTestResultBehavior.Continue;
                                }
                            },
                            new PointHitTestParameters(pt));
                        if (hitStripe != null)
                        {
                            foreach (SelectableStripe managedStripe in this.managedStripes)
                            {
                                managedStripe.IsSelected = false;
                            }

                            hitStripe.IsSelected = true;
                            this.SelectedStripe = hitStripe;
                            this.TimelineSegment.PerformZoom(0);
                        }
                    }
                };
            this.MouseMove += (s, a) =>
                {
                    if (a.LeftButton == MouseButtonState.Pressed)
                    {
                        this.TimelineSegment.CursorTime = this.DescaleX(a.GetPosition(this).X);
                        a.Handled = true;
                    }
                };
            this.MouseWheel += (s, a) =>
                {
                    this.TimelineSegment.PerformZoom(a.Delta > 0 ? 0.1 : -0.1);
                };
            this.SelectedStripe = null;
        }

        /// <summary>
        /// Sets the enumeration of stripes that are covered and managed by the cursor overlay.
        /// </summary>
        public IEnumerable<SelectableStripe> ManagedStripes
        {
            set
            {
                this.managedStripes = value;
            }
        }

        /// <summary>
        /// Sets the container of the selectable stripes covered by the cursor
        /// </summary>
        public UIElement SelectableStripesContainer
        {
            set
            {
                this.selectableStripesContaner = value;
            }
        }

        /// <summary>
        /// Gets the selected stripe.
        /// </summary>
        public SelectableStripe SelectedStripe
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the timeline segment. This version overrides the base property, adding the event
        /// that is fired when the cursor is moved.
        /// </summary>
        public override ITimelineSegment TimelineSegment
        {
            get
            {
                return base.TimelineSegment;
            }

            set
            {
                base.TimelineSegment = value;
                base.TimelineSegment.OnCursorChange += (c) =>
                {
                    CursorPosition = c;
                };
            }
        }

        /// <summary>
        /// Gets or sets the position of the cursor in seconds.
        /// </summary>
        public double CursorPosition
        {
            get
            {
                return this.cursorPosition;
            }

            set
            {
                this.cursorPosition = value;
                this.UpdateComponent();
            }
        }

        /// <summary>
        /// Gets the height of the top stripe in pixels.
        /// </summary>
        public double TopStripeHeight
        {
            get
            {
                // This gets the Attached Proprty
                int row = Grid.GetRow(this);

                Grid parentGrid = this.Parent as Grid;
                if (parentGrid != null)
                {
                    return parentGrid.RowDefinitions[row].ActualHeight;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Adds an auxiliary cursor.
        /// </summary>
        /// <param name="position">Position of the cursor in seconds.</param>
        /// <param name="brush">Brush to be used to render it.</param>
        /// <returns>The auxiliary cursor added.</returns>
        public AuxiliaryCursor AddAuxiliaryCursor(double position, Brush brush)
        {
            AuxiliaryCursor result = new AuxiliaryCursor()
            {
                Position = position,
                CursorBrush = brush
            };
            this.auxiliaryCursors.Add(result);
            return result;
        }

        /// <summary>
        /// Clears all the auxiliary cursors.
        /// </summary>
        public void ClearAuxiliaryCursors()
        {
            this.auxiliaryCursors.Clear();
        }

        /// <summary>
        /// Scales the passed time in absolute time, so that the position in pixels is similar to the relative position in seconds.
        /// </summary>
        /// <param name="x">Time expressed in second.</param>
        /// <returns>Position on screen expressed in pixel.</returns>
        public double AbsoluteScaleX(double x)
        {
            return this.ActualWidth * (x - this.TimelineSegment.MinTime) / (this.TimelineSegment.MaxTime - this.TimelineSegment.MinTime);
        }

        /// <summary>
        /// Redraws the main cursor and the auxiliary cursors
        /// </summary>
        public override void Redraw()
        {
            this.ClipToBounds = true;
            double endTopCursor = this.TopStripeHeight * (StartTopCursor + HeightTopCursor);
            double startTopCursor = this.TopStripeHeight * StartTopCursor;
            double scaledMin = this.AbsoluteScaleX(this.TimelineSegment.MinShownTime);
            double scaledMax = this.AbsoluteScaleX(this.TimelineSegment.MaxShownTime);
            var timeCursor = new TimeCursor(this, StartTopCursor, HeightTopCursor);

            // Draws the portion of shown window with respect to all
            Polyline shownWidget = new Polyline();
            shownWidget.Stroke = Brushes.DarkBlue;
            shownWidget.StrokeThickness = 2;
            shownWidget.Points.Add(new Point(0, endTopCursor));
            shownWidget.Points.Add(new Point(scaledMin, endTopCursor));
            shownWidget.Points.Add(new Point(scaledMin, startTopCursor));
            shownWidget.Points.Add(new Point(scaledMax, startTopCursor));
            shownWidget.Points.Add(new Point(scaledMax, endTopCursor));
            shownWidget.Points.Add(new Point(this.ActualWidth, endTopCursor));
            this.Children.Add(shownWidget);

            // Draws the cursor, the anchor and the axiliary cursors
            if (!double.IsNaN(this.TimelineSegment.AnchorTime))
            {
                this.Children.Add(timeCursor.GetCursor(this.TimelineSegment.AnchorTime, Brushes.Blue));
            }

            this.Children.Add(timeCursor.GetCursor(this.cursorPosition, Brushes.Red));
            foreach (var cursor in this.auxiliaryCursors)
            {
                this.Children.Add(timeCursor.GetCursor(cursor.Position, Brushes.Red));
            }
        }

        /// <summary>
        /// Utility class to describe auxiliary cursors that are not moved by the cursor
        /// </summary>
        public class AuxiliaryCursor
        {
            /// <summary>
            /// Gets or sets the position of the auxiliary cursor in seconds
            /// </summary>
            public double Position
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the brush to be used for the auxiliary cursors
            /// </summary>
            public Brush CursorBrush
            {
                get;
                set;
            }
        }
    }
}
