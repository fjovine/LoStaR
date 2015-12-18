//-----------------------------------------------------------------------
// <copyright file="Stripe.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// A Stripe is a graphic widget that shows some time-dependent value or quantity.
    /// It receives the time information it needs to rebuild itself through a ITimelineSegment interface with which 
    /// it gets the time information and additional events to be handled.
    /// </summary>
    public abstract class Stripe : Canvas
    {
        /// <summary>
        /// Backing property of the TimelineSegment property.
        /// </summary>
        private ITimelineSegment timelineSegment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Stripe" /> class.
        /// </summary>
        public Stripe()
        {
            this.Loaded += (s, a) =>
            {
                this.UpdateComponent();
            };

            // The transparent background i needed. If we do not this way, the events do not get routed correctly.
            this.Background = Brushes.Transparent;
        }

        /// <summary>
        /// Gets or sets the Interface governing the interface with the timeline parameters.
        /// </summary>
        public ITimelineSegment TimelineSegment
        {
            get
            {
                return this.timelineSegment;
            }

            set
            {
                this.timelineSegment = value;
                this.timelineSegment.OnZoom += () => this.UpdateComponent();
            }
        }

        /// <summary>
        /// Gets or sets the digitalTimeline that contains the time when the bit changes its state to be shown online
        /// </summary>
        public DigitalTimeline Timeline
        {
            get;
            set;
        }

        /// <summary>
        /// Method that must be implemented by a derived class to full the control with the requested graphic elements.
        /// </summary>
        public abstract void Redraw();

        /// <summary>
        /// Scales the time to the horizontal position in the stripe.
        /// </summary>
        /// <param name="time">Time in seconds.</param>
        /// <returns>The horizontal position correspondent to the passed value.</returns>
        protected double ScaleX(double time)
        {
            return this.ActualWidth * (time - this.TimelineSegment.MinShownTime) / (this.TimelineSegment.MaxShownTime - this.TimelineSegment.MinShownTime);
        }

        /// <summary>
        /// Computes the position in pixel starting from the logic position.
        /// </summary>
        /// <param name="pixelPosition">Coordinate in pixel.</param>
        /// <returns>Coordinate in logical position.</returns>
        protected double DescaleX(double pixelPosition)
        {
            return ((pixelPosition / this.ActualWidth) * (this.TimelineSegment.MaxShownTime - this.TimelineSegment.MinShownTime)) + this.TimelineSegment.MinShownTime;
        }

        /// <summary>
        /// Scales the state to the available height.
        /// </summary>
        /// <param name="state">Logical level.</param>
        /// <returns>The vertical coordinate of the logical level.</returns>
        protected double ScaleY(bool state)
        {
            return state ? 0 : this.ActualHeight;
        }

        /// <summary>
        /// Adds text to the Stripe in the passed position.
        /// </summary>
        /// <param name="pixelX">X coordinate of the text to show in pixels.</param>
        /// <param name="pixelY">Y coordinate of the text to show in pixels.</param>
        /// <param name="text">Text to show.</param>
        /// <param name="horizontalAlignment">Horizontal alignment of the text with respect to the passed x coordinate.</param>
        /// <param name="fontSize">Font size to be used.</param>
        protected void AddText(double pixelX, double pixelY, string text, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, double fontSize = 8)
        {
            var caption = new TextBlock() { Text = text, FontSize = fontSize, HorizontalAlignment = horizontalAlignment };
            double startX = pixelX;

            this.Children.Add(caption);
            caption.Loaded += (s, a) =>
            {
                switch (horizontalAlignment)
                {
                    case HorizontalAlignment.Right:
                        startX -= caption.ActualWidth;
                        break;
                    case HorizontalAlignment.Center:
                        startX -= caption.ActualWidth / 2;
                        break;
                    default:
                        break;
                }

                Canvas.SetLeft(caption, startX);
                Canvas.SetTop(caption, pixelY);
            };
        }

        /// <summary>
        /// Clears what previously was on the canvas and draws the new content.
        /// </summary>
        protected void UpdateComponent()
        {
            this.Children.Clear();
            this.Redraw();
        }
    }
}
