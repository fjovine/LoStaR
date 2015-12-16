// <copyright file="TimeAxisStripe.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System.Windows.Controls;

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
                    this.AddText(this.ScaleX(x), 0, s);
                });
        }
    }
}
