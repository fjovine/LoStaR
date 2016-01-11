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
    public class ProtocolTimelineTest
    {
        [Test]
        public void Init_CorrectlyBuildsProtocolTimeline_WhenCalledWithSingleSpan()
        {
            ProtocolTimeline protocolTimeline = new ProtocolTimeline();
            SpanTimeline spanTimelineA = createTimeline(10);
            SpanTimeline spanTimelineB = createTimeline(20);
            SpanTimeline spanTimelineC = createTimeline(30);
            protocolTimeline.Add("TimelineA", spanTimelineA);
            protocolTimeline.Add("TimelineB", spanTimelineB);
            protocolTimeline.Add("TimelineC", spanTimelineC);

            protocolTimeline.Init();

            Assert.AreEqual(3, protocolTimeline.Count);
            Assert.AreEqual("TimelineA", protocolTimeline[0].LineIdentifier);
            Assert.AreEqual(10, protocolTimeline[0].LineInfo.TimeStart);
            Assert.AreEqual("TimelineB", protocolTimeline[1].LineIdentifier);
            Assert.AreEqual(20, protocolTimeline[1].LineInfo.TimeStart);
            Assert.AreEqual("TimelineC", protocolTimeline[2].LineIdentifier);
            Assert.AreEqual(30, protocolTimeline[2].LineInfo.TimeStart);
        }

        [TestCase(
            new String[] {"A","A","B","A","B"},
            new double[] {10,20,30,40,50}
            )]
        [TestCase(
            new String[] { "A", "C", "B", "A", "B" },
            new double[] { 10, 20, 30, 40, 50 }
            )]
        [TestCase(
            new String[] { "B", "A", "A", "A", "A" },
            new double[] { 10, 20, 30, 40, 50 }
            )]
        public void Init_CorrectlyBuildsTimeline_WhenCalledWithMultipleSpan(string[] names, double[] times)
        {
            HashSet<string> namesDefined = new HashSet<string>();
            List<SpanTimeline> timelines = new List<SpanTimeline>();
            Dictionary<string, SpanTimeline> map = new Dictionary<string, SpanTimeline>();

            // builds all the timelines
            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i];
                if (!namesDefined.Contains(name))
                {
                    SpanTimeline st = new SpanTimeline();
                    namesDefined.Add(name);
                    map.Add(name, st);
                }
                map[name].Append(new SpanInfo() { TimeStart = times[i], Duration = 1 });
            }

            // buolds the protocol timeline
            ProtocolTimeline protocolTimeline = new ProtocolTimeline();
            foreach (var name in namesDefined)
            {
                protocolTimeline.Add(
                    name,
                    map[name]
                    );
            }
            protocolTimeline.Init();

            int j=0;
            foreach (ProtocolInfo protocol in protocolTimeline.Timeline) 
            {
                Assert.AreEqual(names[j], protocol.LineIdentifier);
                Assert.AreEqual(times[j], protocol.LineInfo.TimeStart);
                j++;
            }
        }

        private SpanTimeline createTimeline(params double[] startTimes)
        {
            SpanTimeline result = new SpanTimeline();
            Array.Sort(startTimes);
            foreach (var start in startTimes)
            {
                result.Append(new SpanInfo()
                {
                    TimeStart = start,
                    Duration = 1
                });
            }
            return result;
        }
    }
}
