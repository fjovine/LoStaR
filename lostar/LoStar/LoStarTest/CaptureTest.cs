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
    public class CaptureTest
    {
        [Test]
        public void LoadFromStream_CorreclyLoads_FakeXml()
        {
            MemoryStream testStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(testCapture));
            var stream = new StreamReader(testStream);
            var capture = Capture.LoadFromStream(stream);

            Assert.AreEqual(121982, capture.TransitionCount);
            Assert.AreEqual(100000, capture.BufferSize);
            Assert.AreEqual(4, capture.TransitionContainer.Count);
        }

        private static string testCapture =
            "<?xml version='1.0' encoding='utf-8'?>\n" +
            "<Capture  xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>\n" +
            "	<TransitionCount>121982</TransitionCount>\n" +
            "	<BufferSize>100000</BufferSize>\n" +
            "	<TransitionContainer>\n" +
            "		<Transition time='33.895548' state='40'/>\n" +
            "		<Transition time='33.897463' state='56'/>\n" +
            "		<Transition time='33.899377' state='40'/>\n" +
            "		<Transition time='33.901293' state='56'/>\n" +
            "	</TransitionContainer>\n" +
            "</Capture>\n";
    }
}
