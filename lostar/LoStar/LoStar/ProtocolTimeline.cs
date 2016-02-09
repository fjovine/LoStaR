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
    using System.Diagnostics;
    using System.IO;
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
        /// Gets the <c>ProtocolInfo</c> at the passed index.
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
        /// Exports in text format the content of the protocol timeline.
        /// </summary>
        /// <param name="filename">Pathname of the file where the timeline is stored.</param>
        /// <param name="bytesPerLine">Number of bytes shown per line. Defaults to 16.</param>
        public void TxtExport(string filename, int bytesPerLine = 16)
        {
            // Builds the index of line numbers
            // The number in the <c>i-th</c> position in this list is the final line number occupied
            // by the byte line of the <c>i-th</c> message of the protocol
            List<int> protocolEntryAtLine = new List<int>();
            if (this.timeline == null)
            {
                this.Init();
            }

            int cumulatedLines = 0;
            protocolEntryAtLine.Add(cumulatedLines);
            this.timeline.ForEach((pi) =>
                {
                    int numBytes = ((List<byte>)pi.LineInfo.Payload).Count;
                    int numLines = numBytes / bytesPerLine;
                    if ((numBytes % bytesPerLine) != 0)
                    {
                        numLines++;
                    }

                    cumulatedLines += numLines;
                    protocolEntryAtLine.Add(cumulatedLines);
                });
            var columnWidth = new int[] 
                {
                    10, // Initial time
                    10, // Distance from previous event
                    10, // Identifier
                    (3 * bytesPerLine) - 1, // Hex representation
                    bytesPerLine // ascii representation
                };
            var document = new FixedSpaceTxtDocument(
                "Protocol",
                columnWidth,
                (line, col) =>
                {
                    bool firstLine = true;
                    int entryNo = protocolEntryAtLine.BinarySearch(line);
                    if (entryNo < 0) 
                    {
                        firstLine = false;
                        entryNo = ~entryNo;
                        entryNo--;
                    }

                    if (entryNo < 0) 
                    {
                        throw new ArgumentException("Meaningless index");
                    }

                    if (entryNo >= this.Count)
                    {
                        return null;
                    }

                    ProtocolInfo previousProtocolInfo = entryNo > 0 ? this[entryNo - 1] : null;
                    ProtocolInfo protocolInfo = this[entryNo];
                    int relativeLine = line - protocolEntryAtLine[entryNo];
                    List<byte> payload = (List<byte>)protocolInfo.LineInfo.Payload; 
                    switch (col)
                    {
                        case 0: // Initial time
                            return firstLine ? "r" + protocolInfo.LineInfo.TimeStart.ToString("0.000000") : string.Empty;
                        case 1: // distance from previous event
                            if (!firstLine || previousProtocolInfo == null)
                            {
                                return string.Empty;
                            }

                            double deltaTime = protocolInfo.LineInfo.TimeStart - previousProtocolInfo.LineInfo.TimeStart;
                            return "r" + deltaTime.ToString("0.000000");
                        case 2:
                            return firstLine ? protocolInfo.LineIdentifier : string.Empty;
                        case 3:
                            int start = relativeLine * bytesPerLine;
                            StringBuilder result = new StringBuilder(3 * bytesPerLine);
                            for (int i = 0; i < bytesPerLine; i++)
                            {
                                if (start + i < payload.Count)
                                {
                                    byte current = payload[start + i];
                                    if (i > 0)
                                    {
                                        result.Append(' ');
                                    }

                                    result.Append(current.ToHex());
                                }
                            }

                            return " " + result.ToString();
                        case 4:
                            start = relativeLine * bytesPerLine;
                            result = new StringBuilder(bytesPerLine);
                            for (int i = 0; i < bytesPerLine; i++)
                            {
                                if (start + i < payload.Count)
                                {
                                    byte current = payload[start + i];
                                    result.Append(current.PrintPrintableOrPoint());
                                }
                            }

                            return " " + result.ToString();
                        default:
                            return string.Empty;
                    }
                });
            using (TextWriter tw = new StreamWriter(filename))
            {
                document.PrintAll(tw);
            }
        }

        /// <summary>
        /// Lazy decoded of the xml code used as template for the protocol descriptor.
        /// </summary>
        /// <param name="time">Time in second when the protocol starts.</param>
        /// <returns>The index of the protocol in the protocol index.</returns>
        public int GetProtocolInfoFollowing(double time)
        {
            ProtocolInfo dummyInfo = new ProtocolInfo(string.Empty, new SpanInfo() { TimeStart = time });
            int result = this.timeline.BinarySearch(dummyInfo);
            if (result < 0)
            {
                result = ~result;
            }

            Debug.WriteLine("Indice " + result);
            return result;
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
