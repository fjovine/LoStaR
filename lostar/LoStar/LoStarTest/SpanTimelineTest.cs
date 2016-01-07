using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoStarTest
{
    using LoStar;
    using NUnit.Framework;
    using System.IO;
    [TestFixture]
    public class SpanTimelineTest
    {
        [TestCase(0.5,1)]
        [TestCase(1, 1)]
        public void Append_ThrowsException_WhenWrongTimeInfoIsAdded(double start, double duration)
        {
            SpanTimeline spanTimeline = new SpanTimeline();

            spanTimeline.Append( new SpanInfo() { TimeStart=0, Duration=1 });

            Assert.Throws<ArgumentException>(() =>
            {
                spanTimeline.Append(new SpanInfo() { TimeStart = start, Duration = duration });
            });
        }

        [TestCase(null, -1000)]
        [TestCase(null, 0)]
        [TestCase(1.0, 1.0)]
        [TestCase(1.0, 1.5)]
        [TestCase(1.0, 1.9999)]
        [TestCase(null, 2)]
        [TestCase(8.0, 8.0)]
        [TestCase(null, 7.9999)]
        [TestCase(8.0, 8.9999)]
        [TestCase(null, 9)]
        [TestCase(null, 1000)]
        public void SpanAt_WorksProperly_WithProperParameters(double? expectedStartTime, double time)
        {
            SpanTimeline spanTimeline = this.CreateSpanTimelineWithSpansOfOneSecond(1, 2.3, 5.5, 8);

            SpanInfo result = spanTimeline.SpanAt(time);

            double? foundStartTime = null;
            if (result != null) 
            {
                foundStartTime = result.TimeStart;
            }

            Assert.AreEqual(expectedStartTime, foundStartTime);
        }

        [TestCase(0,10, new double[] { 1, 2.3, 5.5, 8 })]
        [TestCase(1, 10, new double[] { 1, 2.3, 5.5, 8 })]
        [TestCase(1.5, 10, new double[] { 1, 2.3, 5.5, 8 })]
        [TestCase(2, 10, new double[] { 1, 2.3, 5.5, 8 })]
        [TestCase(2.0001, 10, new double[] { 2.3, 5.5, 8 })]
        [TestCase(2.0001, 8.9999, new double[] { 2.3, 5.5, 8 })]
        [TestCase(2.0001, 8.5, new double[] { 2.3, 5.5, 8 })]
        [TestCase(2.0001, 8, new double[] { 2.3, 5.5, 8 })]
        [TestCase(2.0001, 7.9999, new double[] { 2.3, 5.5 })]
        [TestCase(0, 0.1, new double[] { })]
        [TestCase(9.1, 9.2, new double[] { })]
        public void ForEach_WorksProperly_WithProperParameters(double fromSec, double toSec, double[] expectedStartTimes)
        {
            List<double> foundStartTimes = new List<double>();

            SpanTimeline spanTimeline = this.CreateSpanTimelineWithSpansOfOneSecond(1, 2.3, 5.5, 8);

            spanTimeline.ForEach(fromSec, toSec, (s) =>
                {
                    foundStartTimes.Add(s.TimeStart);
                });

            Assert.AreEqual(expectedStartTimes, foundStartTimes);
        }

        [TestCase(0, true, null)]
        [TestCase(0, false, 1.0)]
        [TestCase(1.0, true, null)]
        [TestCase(1.0, false, 2.0)]
        [TestCase(1.5, true, 1.0)]
        [TestCase(1.5, false, 2.0)]
        [TestCase(2.3, true, 2.0)]
        [TestCase(2.3, false, 3.3)]
        [TestCase(2.1, true, 2.0)]
        [TestCase(2.1, false, 2.3)]
        [TestCase(3.4, true, 3.3)]
        [TestCase(4.5, false, 5.5)]
        [TestCase(8, false, 9.0)]
        [TestCase(8.5, false, 9.0)]
        [TestCase(9, false, null)]
        [TestCase(9.000001, false, null)]
        [TestCase(100, true, 9.0)]
        [TestCase(100, false, null)]
        public void GetNearestEvent_WorksProperly_WithProperParameters(double timeSec, bool isBefore, double? expectedTime)
        {
            SpanTimeline spanTimeline = this.CreateSpanTimelineWithSpansOfOneSecond(1, 2.3, 5.5, 8);

            Assert.AreEqual(expectedTime, spanTimeline.GetNearestEvent(timeSec, isBefore));
        }

        private SpanTimeline CreateSpanTimelineWithSpansOfOneSecond(params double[] startTimes)
        {
            SpanTimeline result = new SpanTimeline();

            foreach (var start in startTimes)
            {
                result.Append(new SpanInfo() { TimeStart = start, Duration = 1 });
            }

            return result;
        }
    }
}
