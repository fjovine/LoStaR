//-----------------------------------------------------------------------
// <copyright file="ProtocolControl.xaml.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
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
        /// Sets the timeline to be represented by the control
        /// </summary>
        public ProtocolTimeline Timeline
        {
            set
            {
                this.ProtocolData.ItemsSource = value.Timeline;
            }
        }
    }
}
