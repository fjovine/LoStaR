//-----------------------------------------------------------------------
// <copyright file="DigitalStripe.cs" company="hiLab">
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
    /// Interaction logic for <c>Stripe.xaml</c>
    /// </summary>
    public class DigitalStripe : Stripe
    {
        /// <summary>
        /// Height of the caption area in pixels.
        /// </summary>
        private double captionHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalStripe" /> class.
        /// </summary>
        public DigitalStripe()
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
        /// Fills the component with the waveform
        /// </summary>
        public override void Redraw()
        {
            bool isFirst = true;
            bool lastState = false;
            double lastWhen = 0;

            if (this.IsSelected)
            {
                var selectedCaption = new Rectangle() { Fill = Brushes.LightGray, Width = this.ActualWidth, Height = this.Margin.Top * 0.7 };
                Canvas.SetLeft(selectedCaption, 0);
                Canvas.SetTop(selectedCaption, -this.Margin.Top * 0.8);
                this.Children.Add(selectedCaption);
            }

            this.AddText(0, -this.Margin.Top, this.Caption);

            this.Timeline.ForEach(
                this.TimelineSegment.MinShownTime,
                this.TimelineSegment.MaxShownTime,
                (state, when) =>
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        this.Children.Add(new Line()
                        {
                            X1 = ScaleX(lastWhen),
                            X2 = ScaleX(when),
                            Y1 = ScaleY(lastState),
                            Y2 = ScaleY(lastState),
                            Stroke = Brushes.Black,
                            StrokeThickness = lastState ? 0.8 : 2.5
                        });
                        this.Children.Add(new Line()
                        {
                            X1 = ScaleX(when),
                            X2 = ScaleX(when),
                            Y1 = ScaleY(lastState),
                            Y2 = ScaleY(state),
                            Stroke = Brushes.LightGray,
                            StrokeThickness = 1
                        });
                    }

                    lastState = state;
                    lastWhen = when;
                });
        }
    }
}
