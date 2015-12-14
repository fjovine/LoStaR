//-----------------------------------------------------------------------
// <copyright file="DigitalStripe.xaml.cs" company="hiLab">
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
    public partial class DigitalStripe : Canvas
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
            this.InitializeComponent();
            this.Loaded += (s, a) =>
            {
                this.FillComponent();
            };
        }

        /// <summary>
        /// Gets or sets the initial time of the shown timespan.
        /// </summary>
        public double InitialTime
        {
            private get;
            set;
        }

        /// <summary>
        /// Gets or sets the final time of the shown timespan.
        /// </summary>
        public double FinalTime
        {
            private get;
            set;
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
        /// Gets or sets the digitalTimeline that contains the time when the bit changes its state to be shown online
        /// </summary>
        public DigitalTimeline Timeline
        {
            get;
            set;
        }

        /// <summary>
        /// Fills the component with the waveform
        /// </summary>
        public void FillComponent()
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

            var caption = new TextBlock() { Text = this.Caption, FontSize = 8 };
            Canvas.SetLeft(caption, 0);
            Canvas.SetTop(caption, -this.Margin.Top);
            this.Children.Add(caption);

            this.Timeline.ForEach(
                this.InitialTime, 
                this.FinalTime,
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
                            X1 = ScaleX(lastWhen), X2 = ScaleX(when), Y1 = ScaleY(lastState), Y2 = ScaleY(lastState),
                            Stroke = Brushes.Black,
                            StrokeThickness = lastState ? 0.8 : 2.5
                        });
                        this.Children.Add(new Line() 
                        {
                            X1 = ScaleX(when), X2 = ScaleX(when), Y1 = ScaleY(lastState), Y2 = ScaleY(state),
                            Stroke = Brushes.LightGray,
                            StrokeThickness = 1
                        });
                    }

                    lastState = state;
                    lastWhen = when;
                });
        }

        /// <summary>
        /// Scales the time to the horizontal position in the stripe.
        /// </summary>
        /// <param name="time">Time in seconds.</param>
        /// <returns>The horizontal position correspondent to the passed value.</returns>
        private double ScaleX(double time)
        {
            return this.ActualWidth * (time - this.InitialTime) / (this.FinalTime - this.InitialTime);
        }

        /// <summary>
        /// Scales the state to the available height.
        /// </summary>
        /// <param name="state">Logical level.</param>
        /// <returns>The vertical coordinate of the logical level.</returns>
        private double ScaleY(bool state)
        {
            return state ? 0 : this.ActualHeight;
        }
    }
}
