//-----------------------------------------------------------------------
// <copyright file="ITimelineSegment.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    /// <summary>
    /// Interface exposing properties and methods used by the visual appearance of a timeline.
    /// In a timeline the time is always expressed in seconds from the beginning. It is possible that not all
    /// the states are available so <c>MinTime</c> is more than 0.
    /// </summary>
    public interface ITimelineSegment
    {
        /// <summary>
        /// Gets the minimum time in seconds from which data is available.
        /// </summary>
        double MinTime
        {
            get;
        }

        /// <summary>
        /// Gets the maximum time in seconds for which data is available.
        /// </summary>
        double MaxTime
        {
            get;
        }

        /// <summary>
        /// Gets or sets the minimum time shown on the timeline.
        /// </summary>
        double MinShownTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum time shown on the timeline.
        /// </summary>
        double MaxTimeShown
        {
            get;
            set;
        }

        /// <summary>
        /// Zooms in the timeline, i.e. shows a more detailed view.
        /// </summary>
        void ZoomIn();

        /// <summary>
        /// Zooms out the timeline, i.e. shows a less detailed view.
        /// </summary>
        void ZoomOut();

        /// <summary>
        /// Shows all the timeline on the available data.
        /// </summary>
        void ZoomAll();
    }
}
