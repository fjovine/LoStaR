//-----------------------------------------------------------------------
// <copyright file="DigitalTimeline.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A Digital timeline describes when a single bit changes its state.
    /// Starting from the initial state, every state transition is stored with the time in seconds of
    /// the transition.
    /// </summary>
    public class DigitalTimeline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalTimeline" /> class.
        /// This constructors scans the captured states and creates a digital timeline containing only the state transitions 
        /// </summary>
        /// <param name="capture">Capture object containing a stripe of captured digital states.</param>
        /// <param name="bit">Number of the bit to be saved in the timeline. 0 is the least significant bit.</param>
        public DigitalTimeline(Capture capture, int bit)
        {
            this.Transitions = new List<double>();
            int mask = 0x1 << bit;
            bool isFirst = true;
            bool lastState = false;

            foreach (var transition in capture.TransitionContainer)
            {
                bool state = (transition.State & mask) != 0;
                if (isFirst)
                {
                    this.InitialState = state;
                    this.Transitions.Add(transition.Time);
                    isFirst = false;
                    lastState = state;
                }
                else
                {
                    if (state != lastState)
                    {
                        this.Transitions.Add(transition.Time);
                        lastState = state;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalTimeline" /> class.
        /// </summary>
        public DigitalTimeline()
        {
            this.Transitions = new List<double>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the state the digital timeline was true or false **after** the first transition has taken place. (Not before!)
        /// </summary>
        public bool InitialState
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an ordered list of times expressed in seconds when the transitions happen.
        /// </summary>
        public List<double> Transitions
        {
            get;
            private set;
        }

        /// <summary>
        /// Factory method that creates a digital timeline storing the passed bytes as serial transition in UART format
        /// at the passed baud rate. The first character is sent at deltaTime, the the others follow at the same
        /// interval.
        /// </summary>
        /// <param name="baud">BaudRate to be generated.</param>
        /// <param name="deltaTime">Time interval between the bytes generated.</param>
        /// <param name="bytes">Bytes to be added to the timeline.</param>
        /// <returns>A DigitalTimeline that simulates the transmission of the passed bytes.</returns>
        public static DigitalTimeline GenerateTimelineUart(int baud, double deltaTime, params byte[] bytes)
        {
            DigitalTimeline result = new DigitalTimeline();

            result.Transitions.Add(0);
            result.InitialState = true;
            double time = deltaTime;
            double bitTime = 1.0 / baud;

            foreach (byte by in bytes)
            {
                double currentTime = time;
                time += deltaTime;
                result.Transitions.Add(currentTime); // H->startBit

                bool lineState = false;
                for (int i = 0; i < 8; i++)
                {
                    currentTime += bitTime;
                    if (((by & (1 << i)) != 0) != lineState)
                    {
                        result.Transitions.Add(currentTime);
                        lineState = !lineState;
                    }
                }

                // Stop bit
                if (!lineState)
                {
                    result.Transitions.Add(currentTime + bitTime);
                }
            }

            return result;
        }

        /// <summary>
        /// Iterator function that iterates through all the transitions that are inside the passed 
        /// interval, i.e. for which t in [fromSec, toSec].
        /// The extremes are contained.
        /// </summary>
        /// <param name="fromSec">Initial time of the interval in seconds.</param>
        /// <param name="toSec">Final time of the interval in seconds.</param>
        /// <param name="visitor">Delegate called for each transition found having two parameters, 
        /// the state of the line and the time when the transition to that state happened.</param>
        /// <returns>The initial state. When no transition happen during the selected interval, it is the constant logic 
        /// state of the line</returns>
        public bool ForEach(double fromSec, double toSec, Action<bool, double> visitor)
        {
            if (fromSec >= toSec)
            {
                throw new ArgumentException("The initial time equals or is more than the final time");
            }

            int startIndex = this.Transitions.BinarySearch(fromSec);
            if (startIndex < 0)
            {
                startIndex = ~startIndex;
                if (startIndex >= this.Transitions.Count)
                {
                    // the start index is after the last transition
                    return this.StateAt(startIndex);
                }
            }

            int endIndex = this.Transitions.BinarySearch(toSec);
            if (endIndex < 0)
            {
                endIndex = ~endIndex;
                if (endIndex <= 0)
                {
                    // the end index is before the first transition
                    return this.StateAt(-1);
                }
            }

            if (visitor != null)
            {
                double lastTransition = fromSec;

                // The visit starts from the intial time if it does not coincide with the first transition
                if (fromSec < this.Transitions[startIndex])
                {
                    visitor(this.StateAt(startIndex - 1), fromSec);
                }

                // Visit the real transitions
                for (int i = startIndex; i < endIndex; i++)
                {
                    lastTransition = this.Transitions[i];
                    visitor(this.StateAt(i), lastTransition);
                }

                // the visit ends with the final time if it does not coincide with the last transition
                if (toSec > lastTransition)
                {
                    visitor(this.StateAt(endIndex), toSec);
                }
            }

            return this.StateAt(fromSec);
        }

        /// <summary>
        /// Computes the state at the passed index
        /// </summary>
        /// <param name="index">Index of the transition. 0 is the initial transition. </param>
        /// <returns>State after the <c>index-th</c> transition</returns>
        public bool StateAt(int index)
        {
            bool result = this.InitialState;
            if (index < 0)
            {
                result = !result;
            }
            else
            {
                if (index >= this.Transitions.Count)
                {
                    if (this.Transitions.Count % 2 == 0)
                    {
                        result = !result;
                    }
                }
                else
                {
                    if (index % 2 != 0)
                    {
                        // If the index is even, the state equals the initial, otherwise it is inverted.
                        result = !result;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Computes the state at the passed time. State is the logical state of the digital timeline.
        /// If the time equals a transition, then the state **following** the transition will be computed.
        /// If the time is before the first transition, the complement of the initial state is returned.
        /// </summary>
        /// <param name="timeSec">Time at which the state must be computed.</param>
        /// <returns>The state of the line at the passed time</returns>
        public bool StateAt(double timeSec)
        {
            int index = this.Transitions.BinarySearch(timeSec);
            if (index < 0)
            {
                index = ~index;
                return this.StateAt(index - 1);
            }
            else
            {
                return this.StateAt(index);
            }
        }

        /// <summary>
        /// Computes the previous or the following transition with respect to the passed time.
        /// Returns null if there is no previous transition and an event before is asked
        /// or if there is no following transition and an event after is asked
        /// </summary>
        /// <param name="timeSec">Time to be used as reference position.</param>
        /// <param name="isBefore">True if the previous transition must be returned.</param>
        /// <returns>Previous or following transition in seconds or null if none available</returns>
        public double? GetNearestTransition(double timeSec, bool isBefore)
        {
            int index = this.Transitions.BinarySearch(timeSec);
            bool found = index >= 0;
            if (index < 0)
            {
                index = ~index - 1;
            }

            if (isBefore)
            {
                if (index <= 0)
                {
                    return null;
                }
                else
                {
                    if (found)
                    {
                        index--;
                    }

                    return this.Transitions[index];
                }
            }
            else
            {
                if (index >= this.Transitions.Count - 1)
                {
                    return null;
                }
                else
                {
                    return this.Transitions[index + 1];
                }
            }
        }
    }
}