//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System.Windows;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow" /> class.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            DigitalTimeline timeline = new DigitalTimeline();
            timeline.InitialState = false;
            for (double transition = 0; transition < 10; transition += 1)
            {
                timeline.Transitions.Add(transition);
            }

            this.Stripe0.Caption = "Stripe 0";
            this.Stripe0.Timeline = timeline;
            this.Stripe0.InitialTime = 0;
            this.Stripe0.FinalTime = 10;

            timeline = new DigitalTimeline();
            for (double transition = 0; transition < 10; transition += 0.5)
            {
                timeline.Transitions.Add(transition);
            }

            this.Stripe1.Caption = "Stripe 1";
            this.Stripe1.IsSelected = true;
            this.Stripe1.Timeline = timeline;
            this.Stripe1.InitialTime = 0;
            this.Stripe1.FinalTime = 10;
        }
    }
}