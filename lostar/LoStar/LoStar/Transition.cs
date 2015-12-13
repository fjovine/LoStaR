//-----------------------------------------------------------------------
// <copyright file="Transition.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System.Xml.Serialization;

    /// <summary>
    /// A transition is a period of time during which the state of the input lines of the LogicStateRecorder are stable.
    /// The information a transition stores is the time, expressed in seconds when the transition happened and
    /// the new state of the sampled lines.
    /// </summary>
    public class Transition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Transition" /> class.
        /// </summary>
        public Transition()
        {
        }

        /// <summary>
        /// Gets or sets the time expressed in seconds from the beginning of the sample acquisition, when the transition takes place.
        /// </summary>
        [XmlAttribute]
        public double Time
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sampled state of the digital lines.
        /// </summary>
        [XmlAttribute]
        public int State
        {
            get;
            set;
        }
    }
}
