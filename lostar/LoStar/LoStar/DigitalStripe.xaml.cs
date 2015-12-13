//-----------------------------------------------------------------------
// <copyright file="DigitalStripe.xaml.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for <c>Stripe.xaml</c>
    /// </summary>
    public partial class Stripe : Canvas
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Stripe" /> class.
        /// </summary>
        public Stripe()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the digitalTimeline that contains the time when the bit changes its state to be shown online
        /// </summary>
        public DigitalTimeline DigitalTimeline
        {
            get;
            set;
        }

        public void Show(double fromTime, double toTime)
        {
        }
    }
}
