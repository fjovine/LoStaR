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
    public class ProtocolInfo
    {
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
    }
}
