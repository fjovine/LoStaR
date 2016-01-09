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
    /// A DigitalStripe is a selectable stripe that shows the waveform of a digital signal, i.e. shows
    /// a rectangle waveform.
    /// </summary>
    public class DigitalStripe : SelectableStripe
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalStripe" /> class.
        /// </summary>
        public DigitalStripe()
        {
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
        /// <param name="pixelDuration">Duration of a single pixel on screen</param>
        public override void RedrawContent(double pixelDuration)
        {
            bool lastState = false;

            // the duration of a pixel
            double lastWhen = 0;
            bool firstInPixel = false;

            this.Timeline.ForEach(
                this.TimelineSegment.MinShownTime,
                this.TimelineSegment.MaxShownTime,
                (state, when) =>
                {
                    if (when - lastWhen < pixelDuration)
                    {
                        if (!firstInPixel)
                        {
                            this.Children.Add(new Line()
                            {
                                X1 = ScaleX(when),
                                X2 = ScaleX(when),
                                Y1 = ScaleY(lastState),
                                Y2 = ScaleY(state),
                                Stroke = Brushes.LightGray,
                                StrokeThickness = 1
                            });
                            firstInPixel = true;
                            lastWhen = when;
                        }
                    }
                    else
                    {
                        firstInPixel = false;
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
                        lastWhen = when;
                    }

                    lastState = state;
                });
        }

        /// <summary>
        /// Computes the previous event with respect to the passed time.
        /// An event is a relevant happening that is completely dependent on the
        /// subclass design decision.
        /// </summary>
        /// <param name="time">Reference time in seconds.</param>
        /// <returns>Null if there is no event before, otherwise the time when the event happens.</returns>
        public override double? GetNearestEventBefore(double time)
        {
            return this.Timeline.GetNearestTransition(time, true);
        }

        /// <summary>
        /// Computes the following event with respect to the passed time.
        /// An event is a relevant happening that is completely dependent on the
        /// subclass design decision.
        /// </summary>
        /// <param name="time">Reference time in seconds.</param>
        /// <returns>Null if there is no event before, otherwise the time when the event happens.</returns>
        public override double? GetNearestEventAfter(double time)
        {
            return this.Timeline.GetNearestTransition(time, false);
        }
    }
}
