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
    public class SpanInfoTest
    {
        [Test]
        public void DurationProperty_throwsException_whenNegative()
        {
            SpanInfo spanInfo = new SpanInfo();

            Assert.Throws<ArgumentException>(() => spanInfo.Duration = -1);
        }
    }
}
