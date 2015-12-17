// <copyright file="TimeAxisStripe.cs" company="hiLab">
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
    /// Interaction logic for <c>TimeAxisStripe.xaml</c>
    /// </summary>
    public class TimeAxisStripe : Stripe
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeAxisStripe" /> class.
        /// </summary>
        public TimeAxisStripe()
        {
            this.Height = 30;
        }

        /// <summary>
        /// Draws the time ticks on the stripe
        /// </summary>
        public override void Redraw()
        {
            var tickGenerator = new AxisTickGenerator(
                this.TimelineSegment.MinShownTime,
                this.TimelineSegment.MaxShownTime,
                this.ActualWidth);
            tickGenerator.ForEachTick(
                (s, x) =>
                {
                    double xScaled = this.ScaleX(x);
                    this.AddText(xScaled, 0, s, HorizontalAlignment.Center);
                    this.Children.Add(
                        new Line()
                        {
                            X1 = xScaled,
                            X2 = xScaled,
                            Y1 = this.Height / 2,
                            Y2 = this.Height,
                            Stroke = Brushes.Black,
                            StrokeThickness = 0.5
                        });
                });
        }
    }
}
