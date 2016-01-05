//-----------------------------------------------------------------------
// <copyright file="IBrowsableTimeline.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    /// <summary>
    /// A <c>browsable</c> timeline is an abstraction of a timeline on which there are significant events that can be browsed.
    /// </summary>
    public interface IBrowsableTimeline
    {
        /// <summary>
        /// Gets the previous event with respect to the passed time.
        /// </summary>
        /// <param name="when">Reference time </param>
        /// <returns>Null if there not exist a following event, the time of the previous event otherwise.</returns>
        double? GetNearestEventBefore(double when);

        /// <summary>
        /// Gets the following event with respect to the passed time.
        /// </summary>
        /// <param name="when">Reference time </param>
        /// <returns>Null if there not exist a following event, the time of the following event otherwise.</returns>
        double? GetNearestEventAfter(double when);
    }
}
