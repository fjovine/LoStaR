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
    public class AxisTickGeneratorTest
    {
        [TestCase(10.45, 11.65, 1000)]
        [TestCase(10.45, 11.65, 500)]
        [TestCase(10.45, 11.65, 100)]
        public void AxisTickGenerator_Test(double min, double max, double wid)
        {
            var a = new AxisTickGenerator(min, max, wid);
            a.ForEachTick((d) =>
            {
                Console.Write(d+" ");
            });
            Console.WriteLine();
        }
    }
}
