//-----------------------------------------------------------------------
// <copyright file="ProtocolTimeline.cs" company="hiLab">
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
    /// Timeline describing the protocol, i.e. the sequence of different SpanTimelines containing
    /// a byte[] as payload ordered by start time
    /// </summary>
    public class ProtocolTimeline 
    {
        /// <summary>
        /// Timeline content expressed as list of ProtocolInfo ordered by starting time.
        /// </summary>
        private List<ProtocolInfo> timeline;

        /// <summary>
        /// List of the timelines involved in the ProtocolTimeline.
        /// </summary>
        private List<SpanTimeline> timelines = new List<SpanTimeline>();

        /// <summary>
        /// Maps the timeline index to its name.
        /// </summary>
        private Dictionary<int, string> timelineMap = new Dictionary<int, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolTimeline" /> class.
        /// </summary>
        public ProtocolTimeline()
        {
        }

        /// <summary>
        /// Gets the list of protocol info.
        /// </summary>
        public IEnumerable<ProtocolInfo> Timeline
        {
            get
            {
                return this.timeline;
            }
        }

        /// <summary>
        /// Gets the number of protocol spans in the timeline.
        /// </summary>
        public int Count
        {
            get
            {
                return this.timeline.Count;
            }
        }

        /// <summary>
        /// Gets the protocolinfo at the passed index.
        /// </summary>
        /// <param name="index">Index of the desired <c>ProtocolInfo</c></param>
        /// <returns>The protocol info at the passed index.</returns>
        public ProtocolInfo this[int index]
        {
            get
            {
                return this.timeline[index];
            }
        }

        /// <summary>
        /// Adds a SpanTimeline identified by a string.
        /// </summary>
        /// <param name="identifier">String identifying the timeline.</param>
        /// <param name="spanTimeline">SpanTimeline to be added to the list of timelines managed.</param>
        public void Add(string identifier, SpanTimeline spanTimeline)
        {
            this.timelineMap.Add(this.timelines.Count, identifier);
            this.timelines.Add(spanTimeline);
        }

        /// <summary>
        /// Initializes the class scanning all the added timelines and inserting the <c>SpanInfos</c> all the timelines in strict increasing
        /// order of Starting time.
        /// </summary>
        public void Init()
        {
            this.timeline = new List<ProtocolInfo>();
            int[] indexes = new int[this.timelineMap.Count];

            while (true)
            {
                int nearest = this.GetNearest(indexes);
                if (nearest == -1)
                {
                    break;
                }

                int index = indexes[nearest]++;
                string id = this.timelineMap[nearest];
                ProtocolInfo protocolInfo = new ProtocolInfo(id, this.timelines[nearest][index]);
                this.timeline.Add(protocolInfo);
            }
        }

        /// <summary>
        /// Returns the index of the timeline that has the nearest span start.
        /// The array passed as parameter is the current list of indexes. 
        /// The algorithm scans the timelines and finds the one having the nearest
        /// index, i.e. that whose element identified by the corresponding index
        /// has the smallest Starting Time.
        /// </summary>
        /// <param name="indexes">Array of indexes for current positions</param>
        /// <returns>The index of the timeline that has the nearest event among the current ones.</returns>
        private int GetNearest(int[] indexes)
        {
            int result = -1;
            double nearest = double.MaxValue;
            for (int i = 0; i < indexes.Length; i++)
            {
                SpanInfo spanInfo = this.timelines[i][indexes[i]];
                if (spanInfo == null)
                {
                    continue;
                }

                double next = spanInfo.TimeStart;
                if (next < nearest)
                {
                    nearest = next;
                    result = i;
                }
            }

            return result;
        }
    }
}
