//-----------------------------------------------------------------------
// <copyright file="ProtocolControl.xaml.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ProtocolControl
    /// </summary>
    public partial class ProtocolControl : UserControl
    {
        /// <summary>
        /// Local buffer of the TimelineSegment property.
        /// </summary>
        private ITimelineSegment timelineSegment;

        /// <summary>
        /// Local buffer of the Timeline property.
        /// </summary>
        private ProtocolTimeline protocolTimeline;

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
            private get 
            {
                return this.timelineSegment;
            }

            set
            {
                this.timelineSegment = value;
                this.timelineSegment.OnCursorChange += (cp) =>
                {
                    if (this.IsSync.IsChecked == true)
                    {
                        Debug.WriteLine(">>> ProtocolControl " + cp);
                        int index = this.Timeline.GetProtocolInfoFollowing(cp);
                        Dispatcher.BeginInvoke(
                            new Action(() =>
                            {
                                this.ProtocolData.SelectRowByIndex(index);
                            }));
                    }
                };
            }
        }

        /// <summary>
        /// Gets or sets the timeline to be represented by the control
        /// </summary>
        public ProtocolTimeline Timeline
        {
            get
            {
                return this.protocolTimeline;
            }

            set
            {
                this.protocolTimeline = value;
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
            var addedCells = e.AddedCells;
            if (addedCells.Count > 0)
            {
                ProtocolInfo protocolInfo = (ProtocolInfo)addedCells[0].Item;
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
                    if (this.IsSync.IsChecked == true)
                    {
                        this.timelineSegment.CenterCursor();
                    }
                }
            }
        }

        /// <summary>
        /// Saves the current protocol as a text file.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void SaveProtocol_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "txt files (*.txt) | *.txt";
            if (saveFileDialog.ShowDialog() == false)
            {
                return;
            }
            this.protocolTimeline.TxtExport(saveFileDialog.FileName);
        }
    }
}
