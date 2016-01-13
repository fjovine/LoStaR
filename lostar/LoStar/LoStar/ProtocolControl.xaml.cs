//-----------------------------------------------------------------------
// <copyright file="ProtocolControl.xaml.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ProtocolControl
    /// </summary>
    public partial class ProtocolControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolControl" /> class.
        /// </summary>
        public ProtocolControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the timeline segment shown in the main pane.
        /// </summary>
        public ITimelineSegment TimelineSegment
        {
            private get;
            set;
        }

        /// <summary>
        /// Sets the timeline to be represented by the control
        /// </summary>
        public ProtocolTimeline Timeline
        {
            set
            {
                this.ProtocolData.ItemsSource = value.Timeline;
            }
        }

        /// <summary>
        /// Handles the display of cursor and payload info when the selected row changes.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ProtocolData_RowChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            List<PayloadBrowserHelper> browserContent = new List<PayloadBrowserHelper>();
            ProtocolInfo protocolInfo = (ProtocolInfo)e.AddedCells[0].Item;
            List<byte> payload = (List<byte>)protocolInfo.LineInfo.Payload;
            for (int address = 0; address < payload.Count; address += 16)
            {
                browserContent.Add(
                    new PayloadBrowserHelper(address, payload, 16));
            }

            this.PayloadBrowser.ItemsSource = browserContent;
            if (this.TimelineSegment != null)
            {
                this.TimelineSegment.CursorTime = protocolInfo.LineInfo.TimeStart;
                this.TimelineSegment.PerformZoom(0);
            }
        }
    }
}
