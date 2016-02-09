//-----------------------------------------------------------------------
// <copyright file="ProtocolInfo.cs" company="hiLab">
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
    /// This class represents the information regarding a protocol considered as the dialog between one or more digital lines.
    /// </summary>
    public class ProtocolInfo : IComparable
    {
        /// <summary>
        /// bytes to be shown per payload line
        /// </summary>
        private readonly int bytesPerLine = 16;

        /// <summary>
        /// Private field backing up the LineIdentifier property.
        /// </summary>
        private string lineIdentifier;

        /// <summary>
        /// Private field backing up the SpanInfo property.
        /// </summary>
        private SpanInfo spanInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolInfo" /> class.
        /// </summary>
        /// <param name="lineIdentifier">Name of the line participating to the protocol.</param>
        /// <param name="spanInfo">SpanInfo connected to the identified line.</param>
        public ProtocolInfo(string lineIdentifier, SpanInfo spanInfo)
        {
            this.lineIdentifier = lineIdentifier;
            this.spanInfo = spanInfo;
        }

        /// <summary>
        /// Gets the string identifying one of the lines participating to the protocol.
        /// </summary>
        public string LineIdentifier
        {
            get
            {
                return this.lineIdentifier;
            }
        }

        /// <summary>
        /// Gets the SpanInfo of the named line participating to the protocol.
        /// </summary>
        public SpanInfo LineInfo
        {
            get
            {
                return this.spanInfo;
            }
        }

        /// <summary>
        /// Gets the payload organized in hex lines
        /// </summary>
        public List<string> HexPayload
        {
            get
            {
                List<byte> payload = (List<byte>)this.LineInfo.Payload;
                List<string> result = new List<string>();
                StringBuilder line = new StringBuilder(3 * this.bytesPerLine);
                for (int i = 0; i < payload.Count; i++)
                {
                    byte current = payload[i];
                    line.Append(current.ToHex());
                    line.Append(' ');
                    if (i % this.bytesPerLine == this.bytesPerLine - 1)
                    {
                        result.Add(line.ToString());
                        line.Clear();
                    }
                }

                if (line.Length > 0)
                {
                    result.Add(line.ToString());
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the first line of the payload in hex
        /// </summary>
        public string HexPayloadFirstLine
        {
            get
            {
                return this.HexPayload[0];
            }
        }

        /// <summary>
        /// Gets the payload organized in <c>ASCII</c> lines
        /// </summary>
        public List<string> AsciiPayload
        {
            get 
            {
                List<byte> payload = (List<byte>)this.LineInfo.Payload;
                List<string> result = new List<string>();
                StringBuilder line = new StringBuilder(this.bytesPerLine);
                for (int i = 0; i < payload.Count; i++)
                {
                    byte current = payload[i];
                    line.Append(current.PrintPrintableOrPoint());
                    if (i % this.bytesPerLine == this.bytesPerLine - 1)
                    {
                        result.Add(line.ToString());
                        line.Clear();
                    }
                }

                if (line.Length > 0)
                {
                    result.Add(line.ToString());
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the first line of the payload in <c>ASCII</c>
        /// </summary>
        public string AsciiPayloadFirstLine
        {
            get
            {
                return this.AsciiPayload[0];
            }
        }

        /// <summary>
        /// Comparer of the ProtocolInfo that returns a negative, zero or positive number 
        /// if the current spanInfo precedes, equals or is after that of the other object.
        /// It returns -1 if the other object is not ProtocolInfo
        /// </summary>
        /// <param name="other">Object to be compared to.</param>
        /// <returns>-1,0 or +1 if this is less, equal or more than other. The comparison is the start time of the span.</returns>
        public int CompareTo(object other)
        {
            ProtocolInfo otherProtocolInfo = other as ProtocolInfo;
            if (other == null)
            {
                return -1;
            }

            return this.spanInfo.CompareTo(otherProtocolInfo.spanInfo);
        }
    }
}
