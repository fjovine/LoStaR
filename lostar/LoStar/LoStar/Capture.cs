//-----------------------------------------------------------------------
// <copyright file="Capture.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// This class contains the captured data from the Logic State Recorder.
    /// </summary>
    [Serializable]
    public class Capture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Capture" /> class.
        /// </summary>
        public Capture()
        {
            this.TransitionContainer = new List<Transition>();
        }

        /// <summary>
        /// Gets or sets the total number of captured transitions.
        /// Not necessarily all of them have been stored.
        /// </summary>
        public int TransitionCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the buffer storing the transitions.
        /// This is the maximum number of transitions that can be found in a capture file.
        /// </summary>
        public int BufferSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of all the transition stored in the buffer
        /// </summary>
        public List<Transition> TransitionContainer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the time of the last transition stored.
        /// </summary>
        public double MaxTime
        {
            get
            {
                int count = this.TransitionCount;
                if (count > this.TransitionContainer.Count)
                {
                    count = this.TransitionContainer.Count;
                }

                return this.TransitionContainer[count - 1].Time;
            }
        }

        /// <summary>
        /// Loads the capture from an xml file that serializes it.
        /// </summary>
        /// <param name="reader">Stream from which the captured data is loaded.</param>
        /// <returns>The captured data in a Capture object.</returns>
        public static Capture LoadFromStream(StreamReader reader)
        {
            Capture result = null;
            XmlSerializer serializer = new XmlSerializer(typeof(Capture));
            try
            {
                using (reader)
                {
                    result = (Capture)serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading capture :" + e);
                Console.WriteLine(e.StackTrace);
                serializer = null;
            }

            return result;
        }
    }
}
