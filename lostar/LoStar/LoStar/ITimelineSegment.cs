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
        /// Zoom event to be supported by the class 
        /// </summary>
        event ZoomHandler OnZoom;

        /// <summary>
        /// CursorChange event to be supported by the class.
        /// </summary>
        event CursorHandler OnCursorChange;

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
        double MaxShownTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the duration of the window in seconds.
        /// </summary>
        double WindowDuration
        {
            get;
        }

        /// <summary>
        /// Gets or sets the time position of the cursor on screen.
        /// </summary>
        double CursorTime
        {
            get;
            set;
        }

        /// <summary>
        /// Zooms the timeline around the cursor, i.e. keeping unchanged the
        /// visual position of the cursor.
        /// </summary>
        /// <param name="factor">If positive contracts the limits (more detail) if negative expands the limits (less details)
        /// of the shown window</param>
        void PerformZoom(double factor);

        /// <summary>
        /// Shows all the timeline on the available data.
        /// </summary>
        void ZoomAll();

        /// <summary>
        /// Scrolls the current window moving to the right or to the left the current
        /// window.
        /// The factor determines how the scroll is performed: a negative value scrolls leftwards (towards
        /// the past) a positive value scrolls rightwards (towards the future).
        /// </summary>
        /// <param name="factor">Determines the scrolling percentage. It ranges from -1 to 1. 
        /// -1 means that the window is scrolled completely to the left
        /// +1 means that the window is scrolled completely to the right</param>
        void Scroll(double factor);
    }
}
