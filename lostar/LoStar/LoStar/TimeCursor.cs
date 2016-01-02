//-----------------------------------------------------------------------
// <copyright file="TimeCursor.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Class that manages time cursor to be shown on the CursorOverlay Grid widget.
    /// </summary>
    public class TimeCursor
    {
        /// <summary>
        /// Reference to the CursorOverlay where this cursor will be shown.
        /// </summary>
        private CursorOverlay cursorOverlay;

        /// <summary>
        /// Vertical position where the cursor in absolute coordinates starts expressed as a percentage of the top stripe pixels. 
        /// It is a number from 0.0 to 1.0
        /// </summary>
        private double startOfAbsoluteCursor;

        /// <summary>
        /// Height of the cursor in absolute coordinates expressed as a percentage of the top stripe pixels. 
        /// It is a number from 0.0 to 1.0
        /// </summary>
        private double heightOfAbsoluteCursor;

        /// <summary>
        /// Thickness of the cursor lines in pixels.
        /// </summary>
        private double cursorThickness;

        /// <summary>
        /// Height of the <c>topstripe</c> managed by the CursorOverlay class, where the window position with respect to the overall time available is shown.
        /// </summary>
        private double topStripeHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeCursor" /> class.
        /// </summary>
        /// <param name="cursorOverlay">CursorOverlay WPF grid where the cursor will be shown.</param>
        /// <param name="startOfAbsoluteCursor">Start of the top cursor as percentage of the top stripe.</param>
        /// <param name="heightOfAbsoluteCursor">Height of the top cursor as percentage of the top stripe.</param>
        /// <param name="thickness">Thickness of the cursor expressed in pixel.</param>
        public TimeCursor(CursorOverlay cursorOverlay, double startOfAbsoluteCursor, double heightOfAbsoluteCursor, double thickness = 0.4)
        {
            this.cursorOverlay = cursorOverlay;
            this.startOfAbsoluteCursor = startOfAbsoluteCursor;
            this.heightOfAbsoluteCursor = heightOfAbsoluteCursor;
            this.cursorThickness = thickness;
            this.topStripeHeight = cursorOverlay.TopStripeHeight;
        }

        /// <summary>
        /// Computes a cursor that is composed by three segment. The topmost is the absolute cursor shown in the top stripe.
        /// The bottom segment is the cursor relative to the shown window.
        /// The segment in between connects the others.
        /// </summary>
        /// <param name="cursorPosition">Position of the cursor (normal or auxiliary) in seconds.</param>
        /// <param name="brush">Brush to draw the cursor.</param>
        /// <returns>A polyline to be drawn on screen that represent the cursor.</returns>
        public Polyline GetCursor(double cursorPosition, Brush brush)
        {
            Polyline result = new Polyline();
            result.Stroke = brush;
            result.StrokeThickness = this.cursorThickness;

            double relativeScaledPosition = this.cursorOverlay.ScaleX(cursorPosition);
            double absoluteScaledPosition = this.cursorOverlay.AbsoluteScaleX(cursorPosition);

            // Absolute cursor, i.e. cursor relative to all the time segment sampled
            result.Points.Add(new Point(absoluteScaledPosition, this.startOfAbsoluteCursor * this.topStripeHeight));
            result.Points.Add(new Point(absoluteScaledPosition, this.topStripeHeight * (this.startOfAbsoluteCursor + this.heightOfAbsoluteCursor)));

            // Relative cursor, i.e. cursor relative to the time segment shown in the window
            result.Points.Add(new Point(relativeScaledPosition, this.topStripeHeight));
            result.Points.Add(new Point(relativeScaledPosition, this.cursorOverlay.ActualHeight));
            return result;
        }
    }
}
