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
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// A SpanStripe is a selectable stripe that shows a list of timespans as rectangles on a stripe.
    /// The Spans are represented by a SpanTimeline.
    /// </summary>
    public class SpanStripe : SelectableStripe
    {
        /// <summary>
        /// Local buffer of the template decoded from xml code.
        /// </summary>
        private ControlTemplate guiTemplate = null;

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
        /// Gets or sets the delegate to interpret and draw the payload.
        /// </summary>
        public SpanDrawer PayloadDrawer
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
                        Button b = new Button()
                        {
                            Width = ScaleX(span.TimeEnd) - ScaleX(span.TimeStart),
                            Height = height,
                            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center
                        };
                        b.Template = this.GetTemplate();
                        
                        Canvas.SetLeft(b, ScaleX(span.TimeStart));
                        Canvas.SetTop(b, top);
                        this.Children.Add(b);
                        if (this.PayloadDrawer != null)
                        {
                            PayloadDrawer(span, b, this);
                        }
                    }
                });
        }

        /// <summary>
        /// Determines the nearest event before the passed time.
        /// An event can be the beginning or the ending of a Span.
        /// </summary>
        /// <param name="time">Reference time to use for the search.</param>
        /// <returns>The time of the nearest event before. Null if it does not exist.</returns>
        public override double? GetNearestEventBefore(double time)
        {
            return this.Timeline.GetNearestEvent(time, true);
        }

        /// <summary>
        /// Determines the nearest event after the passed time.
        /// An event can be the beginning or the ending of a Span.
        /// </summary>
        /// <param name="time">Reference time to use for the search.</param>
        /// <returns>The time of the nearest event after. Null if it does not exist.</returns>
        public override double? GetNearestEventAfter(double time)
        {
            return this.Timeline.GetNearestEvent(time, false);
        }

        /// <summary>
        /// Lazy decoding of the template descriptor in xml for the widget use to signal the 
        /// protocol on the timeline.
        /// </summary>
        /// <returns>The control template to be used by a Button object.</returns>
        private ControlTemplate GetTemplate()
        {
            if (this.guiTemplate == null) 
            {
                this.guiTemplate = (ControlTemplate)XamlReader.Parse(@"
                <ControlTemplate  xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                                  xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                                  TargetType='Button'>
                    <Border CornerRadius='0' Background='Lightgray' BorderThickness='1' Padding='2' BorderBrush='Darkgray'>
                        <ContentPresenter HorizontalAlignment='Center' VerticalAlignment='Center' />
                    </Border>
                </ControlTemplate>");
            }

            return this.guiTemplate;
        }
    }
}
