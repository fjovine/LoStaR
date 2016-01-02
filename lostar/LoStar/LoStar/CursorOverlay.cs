//-----------------------------------------------------------------------
// <copyright file="CursorOverlay.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System.Collections.Generic;
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
                    CursorPosition = this.DescaleX(a.GetPosition(this).X);
                    a.Handled = true;
                };
            this.MouseMove += (s, a) =>
                {
                    if (a.LeftButton == MouseButtonState.Pressed)
                    {
                        CursorPosition = this.DescaleX(a.GetPosition(this).X);
                        a.Handled = true;
                    }
                };
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
                this.TimelineSegment.CursorTime = value;
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

            // Draws the cursor and the axiliary cursors
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
