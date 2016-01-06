//-----------------------------------------------------------------------
// <copyright file="SpanStripe.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// A SpanStripe is a selectable stripe that shows a list of timespans as rectangles on a stripe.
    /// The Spans are represented by a SpanTimeline.
    /// </summary>
    public class SpanStripe : SelectableStripe
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpanStripe" /> class.
        /// </summary>
        public SpanStripe()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanStripe" /> class.
        /// </summary>
        /// <param name="spanTimeline">SpanTimeline to draw the content</param>
        public SpanStripe(SpanTimeline spanTimeline)
        {
            this.Timeline = spanTimeline;
        }

        /// <summary>
        /// Gets or sets the span timeline containing information on the spans to be shown.
        /// </summary>
        public SpanTimeline Timeline
        {
            get;
            set;
        }

        /// <summary>
        /// Fills the component with the needed rectangle
        /// </summary>
        /// <param name="pixelDuration">Duration of a single pixel on screen</param>
        public override void RedrawContent(double pixelDuration)
        {
            double bottom = ScaleY(false);
            double top = ScaleY(true);
            double height = Math.Abs(top - bottom);

            this.Timeline.ForEach(
                this.TimelineSegment.MinShownTime,
                this.TimelineSegment.MaxShownTime,
                (span) =>
                {
                    double x1 = ScaleX(span.TimeStart);
                    if (span.Duration < pixelDuration)
                    {
                        this.Children.Add(new Line()
                        {
                            X1 = x1,
                            X2 = x1,
                            Y1 = bottom,
                            Y2 = top,
                            Stroke = Brushes.LightGray,
                            StrokeThickness = 1
                        });
                    }
                    else
                    {
                        Rectangle r = new Rectangle() 
                        {
                            Width = ScaleX(span.TimeEnd) - ScaleX(span.TimeStart),
                            Height = height
                        };
                        r.StrokeThickness = 1;
                        r.Stroke = Brushes.DarkBlue;
                        Canvas.SetLeft(r, ScaleX(span.TimeStart));
                        Canvas.SetTop(r, top);
                        this.Children.Add(r);
                    }
                });
        }

        public override double? GetNearestEventBefore(double time)
        {
            return null;
        }

        public override double? GetNearestEventAfter(double time)
        {
            return null;
        }
    }
}
