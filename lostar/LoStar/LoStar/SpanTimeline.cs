//-----------------------------------------------------------------------
// <copyright file="SpanTimeline.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Timeline containing time ordered spans.
    /// The Spans are represented by SpanInfo classes and cannot overlap.
    /// </summary>
    public class SpanTimeline
    {
        /// <summary>
        /// The ordered list of spans. The list is ordered by the time span start.
        /// The spans cannot be overlapping but the initial time must be in ascending order.
        /// This means that if S[i] and D[i] are the <c>i-th</c> start time and duration, it must be
        /// S[i]+D[i] less than S[i+1]
        /// </summary>
        private List<SpanInfo> infos = new List<SpanInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanTimeline" /> class.
        /// </summary>
        public SpanTimeline()
        {
        }

        /// <summary>
        /// Clears the list of spans.
        /// </summary>
        public void Clear()
        {
            this.infos.Clear();
        }

        /// <summary>
        /// Appends a span to the list already populated.
        /// If the SpanInfo starts before the ending of the last one, an IllegalArgumentException
        /// is thrown.
        /// </summary>
        /// <param name="spanInfo">SpanInfo of the span to be used</param>
        public void Append(SpanInfo spanInfo)
        {
            if (this.infos.Count == 0)
            {
                this.infos.Add(spanInfo);
                return;
            }

            SpanInfo last = this.infos[this.infos.Count - 1];
            if (spanInfo.TimeStart > last.TimeStart + last.Duration)
            {
                this.infos.Add(spanInfo);
            }
            else
            {
                throw new ArgumentException("The passed time span overlaps the last one");
            }
        }

        /// <summary>
        /// Iterates through all the time spans even partially contained in the passed time
        /// interval.
        /// Partially contained means that only one of the extremes of the span in inside
        /// the passed period.
        /// </summary>
        /// <param name="fromSec">Initial time of the period to be visited in seconds.</param>
        /// <param name="toSec">Final time of the period to be visited in seconds.</param>
        /// <param name="visitor">Delegate having the current SpanInfo as parameter that is applied to 
        /// all the spans contained in the interval.</param>
        public void ForEach(double fromSec, double toSec, Action<SpanInfo> visitor)
        {
            int startIndex = this.infos.BinarySearch(new SpanInfo() { TimeStart = fromSec });

            if (startIndex < 0)
            {
                // Not found, check if the previous span contains the start
                startIndex = ~startIndex - 1;
                if (startIndex < 0)
                {
                    startIndex++;
                }
                else
                {
                    if (this.infos[startIndex].TimeEnd < fromSec)
                    {
                        startIndex++;
                    }
                }
            }

            while (startIndex < this.infos.Count)
            {
                SpanInfo current = this.infos[startIndex++];
                if (current.TimeStart <= toSec)
                {
                    visitor(current);
                }
            }
        }

        /// <summary>
        /// Computes the span that is active at the passed time.
        /// </summary>
        /// <param name="timeSec">Reference time to be used.</param>
        /// <returns>The Span active at a passed time if there is one, null otherwise. </returns>
        public SpanInfo SpanAt(double timeSec)
        {
            int index = this.infos.BinarySearch(new SpanInfo() { TimeStart = timeSec });
            if (index >= 0)
            {
                // the Span has been found
                return this.infos[index];
            }

            // The element has not been found, it could be that the time is during the previous element
            index = ~index - 1;
            if (index < 0)
            {
                return null;
            }

            SpanInfo result = this.infos[index];
            if (result.TimeEnd > timeSec)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Computes the previous or the following event with respect to the passed time.
        /// An event is either the start or the end of a span.
        /// Returns null if there is no previous event and an event before is asked
        /// or if there is no following event and an event after is asked
        /// </summary>
        /// <param name="timeSec">Time to be used as reference position.</param>
        /// <param name="isBefore">True if the previous event must be returned.</param>
        /// <returns>Previous or following event in seconds or null if none available</returns>
        public double? GetNearestEvent(double timeSec, bool isBefore)
        {
            if (timeSec < this.infos[0].TimeStart)
            {
                if (isBefore)
                {
                    return null;
                }
                else
                {
                    return this.infos[0].TimeStart;
                }
            }

            if (timeSec > this.infos[this.infos.Count - 1].TimeEnd)
            {
                if (isBefore)
                {
                    return this.infos[this.infos.Count - 1].TimeEnd;
                }
                else
                {
                    return null;
                }
            }

            int index = 0;

            SpanInfo result = this.SpanAt(timeSec, out index);

            if (result != null)
            {
                // we are during a span
                return isBefore ? result.TimeStart : result.TimeEnd;
            }
            else
            {
                // we are between spans
                return isBefore ? this.infos[index].TimeEnd : this.infos[index + 1].TimeStart;
            }
        }

        /// <summary>
        /// Computes the span that is active at the passed time.
        /// </summary>
        /// <param name="timeSec">Reference time to be used.</param>
        /// <param name="index">The index if the span if timeSec is in a Span or that preceding it.</param>
        /// <returns>The Span active at a passed time if there is one, null otherwise. </returns>
        private SpanInfo SpanAt(double timeSec, out int index)
        {
            index = this.infos.BinarySearch(new SpanInfo() { TimeStart = timeSec });
            if (index >= 0)
            {
                // the Span has been found
                return this.infos[index];
            }

            // The element has not been found, it could be that the time is during the previous element            
            index = ~index - 1;
            if (index < 0)
            {
                return null;
            }

            SpanInfo result = this.infos[index];
            if (result.TimeEnd > timeSec)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
