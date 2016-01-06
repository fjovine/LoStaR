//-----------------------------------------------------------------------
// <copyright file="SelectableStripe.cs" company="hiLab">
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
    /// A selectable stripe is a stripe that can be selected double clicking on it
    /// and contains a reference points where to move the cursor to find the 
    /// following or the previous event event.
    /// </summary>
    public abstract class SelectableStripe : Stripe, IBrowsableTimeline
    {
        /// <summary>
        /// Height of the caption area in pixels.
        /// </summary>
        private double captionHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectableStripe" /> class.
        /// </summary>
        public SelectableStripe()
        {
            this.CaptionHeight = 15;
            this.Height = 40;
        }

        /// <summary>
        /// Gets or sets the stripe caption, i.e. a string that describes its content.
        /// </summary>
        public string Caption
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the DigitalStripe is selected.
        /// </summary>
        public bool IsSelected
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height of the caption area in pixel.
        /// </summary>
        public double CaptionHeight
        {
            get
            {
                return this.captionHeight;
            }

            set
            {
                this.captionHeight = value;
                this.Margin = new Thickness(0, value, 0, 0);
            }
        }

        /// <summary>
        /// Computes the previous event with respect to the passed time.
        /// An event is a relevant happening that is completely dependent on the
        /// subclass design decision.
        /// </summary>
        /// <param name="time">Reference time in seconds.</param>
        /// <returns>Null if there is no event before, otherwise the time when the event happens.</returns>
        public abstract double? GetNearestEventBefore(double time);

        /// <summary>
        /// Computes the following event with respect to the passed time.
        /// An event is a relevant happening that is completely dependent on the
        /// subclass design decision.
        /// </summary>
        /// <param name="time">Reference time in seconds.</param>
        /// <returns>Null if there is no event before, otherwise the time when the event happens.</returns>
        public abstract double? GetNearestEventAfter(double time);

        /// <summary>
        /// Redraws the client area of the stripe.
        /// </summary>
        /// <param name="pixelDuration">Duration of a single pixel on screen</param>
        public abstract void RedrawContent(double pixelDuration);

        /// <summary>
        /// A Selectable stripe is composed by a caption that is highlighted when selected and a client area redraw by RedrawContent.
        /// This method draws the caption and calls RedrawComponent.
        /// </summary>
        public override void Redraw()
        {
            if (this.IsSelected)
            {
                var selectedCaption = new Rectangle() { Fill = Brushes.LightGray, Width = this.ActualWidth, Height = this.Margin.Top * 0.7 };
                Canvas.SetLeft(selectedCaption, 0);
                Canvas.SetTop(selectedCaption, -this.Margin.Top * 0.8);
                this.Children.Add(selectedCaption);
            }

            this.AddText(0, -this.Margin.Top, this.Caption);
            this.RedrawContent(this.TimelineSegment.WindowDuration / this.ActualWidth);
        }
    }
}