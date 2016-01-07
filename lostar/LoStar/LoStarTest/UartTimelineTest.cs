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
    public class UartTimelineTest
    {
        [TestCase(new byte[] { 1,2,3,4,5,6,7,8,234 })]
        public void UartTimeline_TheConstructor_Succeeds(params byte[] expectedData)
        {
            List<byte> foundData = new List<byte>();

            DigitalTimeline synteticTimeline = DigitalTimeline.GenerateTimelineUart(9600, 1.5, 1.5, expectedData);

            UartTimeline usartTimeline = new UartTimeline(synteticTimeline, 9600);

            usartTimeline.ForEach(0, 15, (si) =>
            {
                foundData.Add((byte)si.Payload);
            });
            Assert.AreEqual(expectedData, foundData);
        }
    }
}
