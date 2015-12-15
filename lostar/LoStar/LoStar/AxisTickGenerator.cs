using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoStar
{
    public class AxisTickGenerator
    {
        private const double minimumTickDistance = 50;

        private double scaleFactor;

        private double minTick;

        private double maxTick;

        private double deltaTick;

        private int computeNuberOfTicks(double tentativeDelta)
        {
            int result = (int)((Max - Min) / tentativeDelta);
            return result;
        }

        private double distanceBetweenTicks(double TentativeDelta)
        {
            double result = TentativeDelta * scaleFactor;
            return result;
        }

        public AxisTickGenerator(double min, double max, double axisLength)
        {
            Min = Math.Min(min, max);
            Max = Math.Max(min, max);

            double delta = Max-Min;

            scaleFactor = axisLength / delta;

            double order = Math.Floor(Math.Log10(delta));
            deltaTick = Math.Pow(10, order);

            int nextDivisor = 5;

            while (computeNuberOfTicks(deltaTick) < 10 && distanceBetweenTicks(deltaTick) > minimumTickDistance)
            {
                deltaTick /= nextDivisor;
                nextDivisor = (nextDivisor == 5) ? 2 : 5;
            }

            minTick = Math.Ceiling(Min / deltaTick) * deltaTick;
            maxTick = Math.Floor(Max / deltaTick) * deltaTick;
        }

        public void ForEachTick(Action<string> visitor)
        {
            int decimalsCount = (int)Math.Floor(Math.Log10(this.deltaTick));
            Console.WriteLine("{0} - {1} - {2}", decimalsCount, this.deltaTick, Math.Log10(this.deltaTick));
            string format = "N";
            if (decimalsCount > 0) 
            {
                format += "0";
            } else {
                decimalsCount = -decimalsCount;
                format += decimalsCount;
            }
            
            for (double tick = minTick; tick <= maxTick; tick += this.deltaTick)
            {
                visitor(tick.ToString(format));
            }
        }

        public double Min
        {
            get;
            private set;
        }

        public double Max
        {
            get;
            private set;
        }
    }
}
