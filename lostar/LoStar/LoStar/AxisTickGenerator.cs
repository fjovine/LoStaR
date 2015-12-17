//-----------------------------------------------------------------------
// <copyright file="AxisTickGenerator.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Generates the ticks on a graphical axis. It can be used for many different kinds of graphics.
    /// The logical coordinate is the coordinate expressed in logical units. These coordinate
    /// map to the physical space of pixels. This class generate the most graphically pleasing
    /// ticks for an axis.
    /// </summary>
    public class AxisTickGenerator
    {
        /// <summary>
        /// Minimum distance between two ticks in pixels.
        /// </summary>
        private const double MinimumTickDistance = 50;

        private const double MinimumTickTolerance = 0.8;

        /// <summary>
        /// Scale factor between logical coordinate and pixels for the axis.
        /// </summary>
        private double scaleFactor;

        /// <summary>
        /// The tick having the minimum logical coordinate.
        /// </summary>
        private double minTick;

        /// <summary>
        /// The tick having the maximum logical coordinate.
        /// </summary>
        private double maxTick;

        /// <summary>
        /// Distance between two subsequent ticks.
        /// </summary>
        private double deltaTick;

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisTickGenerator" /> class.
        /// </summary>
        /// <param name="min">Minimum logical coordinate of the scale.</param>
        /// <param name="max">Maximum logical coordinate of the scale.</param>
        /// <param name="axisLength">Physical length of the axis in pixels.</param>
        public AxisTickGenerator(double min, double max, double axisLength)
        {
            this.Min = Math.Min(min, max);
            this.Max = Math.Max(min, max);

            double delta = this.Max - this.Min;

            this.scaleFactor = axisLength / delta;

            double order = Math.Floor(Math.Log10(delta));
            this.deltaTick = Math.Pow(10, order);

            int nextDivisor = 5;
            int lastDivisor = 5;

            while (this.ComputeNuberOfTicks(this.deltaTick) < 10 && this.DistanceBetweenTicks(this.deltaTick) > MinimumTickDistance)
            {
                this.deltaTick /= nextDivisor;
                lastDivisor = nextDivisor;
                nextDivisor = (nextDivisor == 5) ? 2 : 5;
            }

            if (this.DistanceBetweenTicks(this.deltaTick) < MinimumTickDistance * MinimumTickTolerance)
            {
                this.deltaTick *= lastDivisor;
            }
            this.minTick = Math.Ceiling(this.Min / this.deltaTick) * this.deltaTick;
            this.maxTick = Math.Floor(this.Max / this.deltaTick) * this.deltaTick;
        }

        /// <summary>
        /// Gets the minimum range of the axis.
        /// </summary>
        public double Min
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the maximum range of the axis.
        /// </summary>
        public double Max
        {
            get;
            private set;
        }

        /// <summary>
        /// Iterator that applies a visitor delegate to each generated tick.
        /// The delegate receives the tick value as a correctly formatted string,
        /// and the tick position.
        /// </summary>
        /// <param name="visitor">Delegate that is applied to every generated tick.</param>
        public void ForEachTick(Action<string, double> visitor)
        {
            int decimalsCount = (int)Math.Floor(Math.Log10(this.deltaTick));
            Console.WriteLine("{0} - {1} - {2}", decimalsCount, this.deltaTick, Math.Log10(this.deltaTick));
            string format = "N";
            if (decimalsCount > 0) 
            {
                format += "0";
            }
            else
            {
                decimalsCount = -decimalsCount;
                format += decimalsCount;
            }

            for (double tick = this.minTick; tick <= this.maxTick; tick += this.deltaTick)
            {
                visitor(tick.ToString(format), tick);
            }
        }

        /// <summary>
        /// Counts the number of ticks contained in the axis.
        /// </summary>
        /// <param name="tentativeDelta">Delta between ticks in logical units (not in pixels).</param>
        /// <returns>The count of ticks generated on the axis.</returns>
        private int ComputeNuberOfTicks(double tentativeDelta)
        {
            int result = (int)((this.Max - this.Min) / tentativeDelta);
            return result;
        }

        /// <summary>
        /// Computes the distance in pixel between two ticks.
        /// </summary>
        /// <param name="tentativeDelta">Delta between ticks in logical units (not in pixels).</param>
        /// <returns>Returns the distance in pixel between two ticks.</returns>
        private double DistanceBetweenTicks(double tentativeDelta)
        {
            double result = tentativeDelta * this.scaleFactor;
            return result;
        }
    }
}
