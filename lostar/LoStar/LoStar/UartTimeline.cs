//-----------------------------------------------------------------------
// <copyright file="UartTimeline.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    /// <summary>
    /// This timeline considers the passed digital timeline as the digital data transmitted to a UART
    /// (Universal Asynchronous Receiver Transmitter) and decodes each single character transmitted
    /// supposing a known baud rate.
    /// A High to Low transition is searched and it is considered a start bit. From this transition
    /// a half bit duration offset is computed and then all the bits are decoded
    /// </summary>
    public class UartTimeline : SpanTimeline
    {
        /// <summary>
        /// Number of bits composing the serial signal decoded.
        /// </summary>
        private int serialBits = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="UartTimeline" /> class.
        /// </summary>
        /// <param name="digitalTimeline">DigitalTimeline that contains the serial data to be decoded.</param>
        /// <param name="baud">Baud rate at which the serial data is sent.</param>
        public UartTimeline(DigitalTimeline digitalTimeline, int baud)
        {
            if (digitalTimeline.Transitions.Count > this.serialBits + 3)
            {
                // Here at least a character is available (start bit, bits stop bit and tolerance)
                // If the initial state is low, we start from the following transition (H->L)
                double currentTime =
                    digitalTimeline.InitialState ?
                    digitalTimeline.Transitions[0] :
                    digitalTimeline.Transitions[1];

                double bitDuration = 1.0 / baud;

                while (true)
                {
                    // Start bit, must be low
                    if (!digitalTimeline.StateAt(currentTime + (bitDuration / 2)))
                    {
                        byte interpretedChar = 0;

                        for (int bit = 0; bit < this.serialBits; bit++)
                        {
                            if (digitalTimeline.StateAt(currentTime + (bitDuration / 2) + ((bit + 1) * bitDuration)))
                            {
                                // the bit is high, sets the corresponding bit
                                interpretedChar |= (byte)(1 << bit);
                            }
                        }

                        if (digitalTimeline.StateAt(currentTime + (bitDuration / 2) + ((this.serialBits + 1) * bitDuration)))
                        {
                            // The stop bit is high, the byte can be stored.
                            this.Append(new SpanInfo()
                            {
                                TimeStart = currentTime,
                                Duration = (this.serialBits + 2) * bitDuration,
                                Payload = interpretedChar
                            });
                        }
                    }

                    currentTime += ((this.serialBits + 2) * bitDuration) + (0.25 * bitDuration);
                    double? nextTransition = digitalTimeline.GetNearestTransition(currentTime, false);
                    if (nextTransition == null)
                    {
                        break;
                    }

                    currentTime = nextTransition.Value;
                }
            }
        }
    }
}
