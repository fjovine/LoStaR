//-----------------------------------------------------------------------
// <copyright file="SpanInfo.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;

    /// <summary>
    /// A Span is a timespan, i.e. an activity that is not instantaneous and lasts a finite
    /// duration. Each span consists of a TimeStart, a Duration and a Payload that can
    /// be tailored to the specific needs of the Span to be described.
    /// </summary>
    public class SpanInfo : IComparable<SpanInfo>
    {
        /// <summary>
        /// Local copy of the span duration in second.
        /// </summary>
        private double duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanInfo" /> class.
        /// </summary>
        public SpanInfo()
        {
        }

        /// <summary>
        /// Gets or sets the initial time of the span expressed in seconds.
        /// </summary>
        public double TimeStart
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the ending time of the span.
        /// </summary>
        public double TimeEnd
        {
            get
            {
                return this.TimeStart + this.duration;
            }
        }

        /// <summary>
        /// Gets or sets the duration of the Span expressed in seconds.
        /// Durations must be positive and a negative or zero duration throws an ArgumentException
        /// </summary>
        public double Duration
        {
            get
            {
                return this.duration;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("The duration cannot be negative");
                }
                else
                {
                    this.duration = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the payload of the Scan.
        /// This information can be used for different purposes and is simply a reference
        /// to an arbitrary object.
        /// </summary>
        public object Payload
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation of the IComparable interface.
        /// </summary>
        /// <param name="other">Other SpanInfo to be compared.</param>
        /// <returns>Returns a positive, zero or negative number if this follows, is equal to or precedes other.</returns>
        public int CompareTo(SpanInfo other)
        {
            if (this.TimeStart < other.TimeStart)
            {
                return -1;
            }
            else if (this.TimeStart > other.TimeStart)
            {
                return 1;
            }

            return 0;
        }
    }
}
