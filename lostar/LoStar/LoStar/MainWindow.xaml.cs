﻿//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Microsoft.Win32;

    /// <summary>
    /// Delegate used to manage the Zoom event.
    /// </summary>
    public delegate void ZoomHandler();

    /// <summary>
    /// Delegate used to manage the cursor movement.
    /// </summary>
    /// <param name="cursorPosition">New position of the cursor in seconds.</param>
    public delegate void CursorHandler(double cursorPosition);

    /// <summary>
    /// Delegate that draws the payload of the a SpanInfo.
    /// </summary>
    /// <param name="spanInfo">The span info to be drawn</param>
    /// <param name="r">Button graphically representing the Span on the stripe</param>
    /// <param name="stripe">SpanStripe where the comment must be drawn</param>
    public delegate void SpanDrawer(SpanInfo spanInfo, Button r, SpanStripe stripe);

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow" /> class.
    /// </summary>
    public partial class MainWindow : Window, ITimelineSegment
    {
        /// <summary>
        /// Baud rate used throughout the application
        /// </summary>
        private const int BaudRate = 9600;

        /// <summary>
        /// Position of the cursor when the last time the mouse was clicked.
        /// It has been added to block frequent cursor movements always on the same time.
        /// </summary>
        private double lastCursorFired = double.NaN;

        /// <summary>
        /// Local repository of the cursor time.
        /// </summary>
        private double cursorTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            Rect rc = GuiUtil.LoadMainWindowPositionFromRegistry();
            this.Left = rc.X;
            this.Top = rc.Y;
            this.Width = rc.Width;
            this.Height = rc.Height;

            this.InitializeComponent();
            this.ToolBar.TimelineSegment = this;
            this.StripeContainer.SizeChanged += (s, a) =>
                {
                    if (this.OnZoom != null)
                    {
                        OnZoom();
                    }
                };

            string[] commandLineArgs = Environment.GetCommandLineArgs();
            string filename = "capture.xml";
            if (commandLineArgs.Length > 1)
            {
                filename = commandLineArgs[2];
                if (!File.Exists(filename))
                {
                    MessageBox.Show(string.Format("The file [{0}] does not exist"));
                    Application.Current.Shutdown();
                }
            }
            else
            {
                string lastInitialFolder = WindowsRegistry.Get(WindowsRegistryEntry.THELASTFOLDER);
                OpenFileDialog fileDialog = new OpenFileDialog();
                if (!Directory.Exists(lastInitialFolder))
                {
                    // if the folder does not exist any more (flash pen extracted) then it defaults to the current folder.
                    lastInitialFolder = null;
                }

                if (lastInitialFolder == null)
                {
                    // If the initial folder is not stored in the registry, gets the current folder
                    fileDialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                }
                else
                {
                    // Otherwise that stored in the registry
                    fileDialog.InitialDirectory = lastInitialFolder;
                }

                fileDialog.Title = "Select capture file";
                fileDialog.Filter = "Capture file|*.xml";
                fileDialog.Multiselect = false;
                if (fileDialog.ShowDialog() == false)
                {
                    return;
                }

                // Stores the current folder in the registry for future reuse.
                WindowsRegistry.Set(
                    WindowsRegistryEntry.THELASTFOLDER,
                    System.IO.Path.GetDirectoryName(fileDialog.FileName));
                filename = fileDialog.FileName;
            }

            Capture capture = Capture.LoadFromStream(new StreamReader(filename));
            this.MinTime = 0;
            this.MaxTime = capture.MaxTime;
            this.MinShownTime = 0;
            this.MaxShownTime = this.MaxTime;

            SpanDrawer uartSpanDrawer = (span, button, stripe) =>
                {
                    List<byte> payload = (List<byte>)span.Payload;
                    StringBuilder hexCaption = new StringBuilder(3 * payload.Count);
                    StringBuilder asciiCaption = new StringBuilder(2 + payload.Count);

                    asciiCaption.Append("'");
                    foreach (byte by in payload)
                    {
                        hexCaption.Append(by.ToString("X"));
                        hexCaption.Append(' ');
                        //asciiCaption.Append((char)by);
                        asciiCaption.Append(by.PrintPrintableOrPoint());
                    }

                    asciiCaption.Append("'");
                    double bitDuration = 1.0 / BaudRate;
                    double bitTime = bitDuration / 2;

                    button.Content = asciiCaption.ToString() + "\n" + hexCaption.ToString();
                    button.ToolTip = hexCaption.ToString();
                    // TODO Implement ToolTip action. Now it is shadowed by CursorOverlay
                };

            /*
            SelectableStripe[] digitalStripes = new SelectableStripe[]
            {
                this.Stripe0,
                this.Stripe1,
                this.Stripe2,
                this.Stripe3,
                this.Stripe4,
                this.Stripe5,
                this.Stripe6,
                this.Stripe7,
                this.StripeUart,
                this.SpanUart
            };

            for (int bit = 0; bit < 8; bit++)
            {
                DigitalTimeline digitalTimeline = new DigitalTimeline(capture, bit);
                if (bit == 0)
                {
                    this.TimeAxis.TimelineSegment = this;
                    this.CursorCanvas.TimelineSegment = this;
                }

                DigitalStripe stripe = (DigitalStripe)digitalStripes[bit];
                stripe.Caption = "Stripe " + bit;
                stripe.Timeline = digitalTimeline;
                stripe.TimelineSegment = this;
            }*/

            this.TimeAxis.TimelineSegment = this;
            this.CursorCanvas.TimelineSegment = this;

            DigitalTimeline timeline0 = new DigitalTimeline(capture, 5);
            this.Stripe0.Caption = "Stripe 0";
            this.Stripe0.Timeline = timeline0;
            this.Stripe0.TimelineSegment = this;

            UartTimeline uartTimeline0 = new UartTimeline(timeline0, BaudRate, true);
            this.Uart0.Caption = "Uart 0";
            this.Uart0.Timeline = uartTimeline0;
            this.Uart0.TimelineSegment = this;
            this.Uart0.PayloadDrawer = uartSpanDrawer;

            DigitalTimeline timeline1 = new DigitalTimeline(capture, 6);
            this.Stripe1.Caption = "Stripe 1";
            this.Stripe1.Timeline = timeline1;
            this.Stripe1.TimelineSegment = this;

            UartTimeline uartTimeline1 = new UartTimeline(timeline1, BaudRate, true);
            this.Uart1.Caption = "Uart 1";
            this.Uart1.Timeline = uartTimeline1;
            this.Uart1.TimelineSegment = this;
            this.Uart1.PayloadDrawer = uartSpanDrawer;

            DigitalTimeline timeline2 = new DigitalTimeline(capture, 7);
            this.Stripe2.Caption = "Stripe 2";
            this.Stripe2.Timeline = timeline2;
            this.Stripe2.TimelineSegment = this;

            UartTimeline uartTimeline2 = new UartTimeline(timeline2, BaudRate);
            this.Uart2.Caption = "Uart 2";
            this.Uart2.Timeline = uartTimeline2;
            this.Uart2.TimelineSegment = this;
            this.Uart2.PayloadDrawer = uartSpanDrawer;

            this.CursorCanvas.ManagedStripes = new SelectableStripe[] 
            {
                this.Stripe0,
                this.Uart0,
                this.Stripe1,
                this.Uart1,
                this.Stripe2,
            };
            this.CursorCanvas.SelectableStripesContainer = this.Stripes;
            this.AnchorTime = double.NaN;

            ProtocolTimeline protocolTimeline = new ProtocolTimeline();
            protocolTimeline.Add("Uart 0", uartTimeline0);
            protocolTimeline.Add("Uart 1", uartTimeline1);
            protocolTimeline.Add("Uart 2", uartTimeline2);
            protocolTimeline.TxtExport("Protocol.txt");

            this.DecodedProtocol.Timeline = protocolTimeline;
            this.DecodedProtocol.TimelineSegment = this;
        }

        /// <summary>
        /// Event triggered following a zoom request.
        /// </summary>
        public event ZoomHandler OnZoom;

        /// <summary>
        /// Event triggered by a cursor movement.
        /// </summary>
        public event CursorHandler OnCursorChange;

        /// <summary>
        /// Gets or sets the minimum time in seconds for which data is available.
        /// </summary>
        public double MinTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum time in seconds for which data is available.
        /// </summary>
        public double MaxTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum time shown on the timeline.
        /// </summary>
        public double MinShownTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum time shown on the timeline.
        /// </summary>
        public double MaxShownTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the duration in second of the window.
        /// </summary>
        public double WindowDuration
        {
            get
            {
                return this.MaxShownTime - this.MinShownTime;
            }
        }

        /// <summary>
        /// Gets or sets the time position of the cursor on screen in seconds.
        /// </summary>
        public double CursorTime
        {
            get
            {
                return this.cursorTime;
            }

            set 
            {
                this.cursorTime = value;
                if (this.OnCursorChange != null && this.lastCursorFired != value)
                {
                    this.OnCursorChange(value);
                    this.lastCursorFired = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the additional reference time called anchor.
        /// </summary>
        public double AnchorTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the delta time between the cursor and the anchor.
        /// </summary>
        public double DeltaTime
        {
            get
            {
                return this.cursorTime - this.AnchorTime;
            }
        }

        /// <summary>
        /// Gets the <c>browsable</c> timeline corresponding to the selected stripe, or null if no stripe is selected.
        /// </summary>
        public IBrowsableTimeline SelectedBrowsableTimeline
        {
            get 
            {
                return this.CursorCanvas.SelectedStripe;
            }
        }

        /// <summary>
        /// Shows all the timeline on the available data.
        /// </summary>
        public void ZoomAll()
        {
            this.MinShownTime = this.MinTime;
            this.MaxShownTime = this.MaxTime;
            this.OnZoom();
        }

        /// <summary>
        /// Computes the new extremes of the window so that the cursor stays where it is and the scale factor is changed according to the passed parameter.
        /// </summary>
        /// <param name="factor">Zoom factor used to compute the new extremes of the window.</param>
        public void PerformZoom(double factor)
        {
            double deltaBefore = this.CursorTime - this.MinShownTime;
            double deltaAfter = this.MaxShownTime - this.CursorTime;

            double tentativeMin = this.MinShownTime + (deltaBefore * factor);
            double tentativeMax = this.MaxShownTime - (deltaAfter * factor);

            if (tentativeMin >= this.MinTime && tentativeMax <= this.MaxTime)
            {
                this.MinShownTime = tentativeMin;
                this.MaxShownTime = tentativeMax;
                this.OnZoom();
            }
        }

        /// <summary>
        /// Scrolls the current window moving to the right or to the left the current
        /// window.
        /// The factor determines how the scroll is performed: a negative value scrolls leftwards (towards
        /// the past) a positive value scrolls rightwards (towards the future).
        /// </summary>
        /// <param name="factor">Determines the scrolling percentage. It ranges from -1 to 1. 
        /// -1 means that the window is scrolled completely to the left
        /// +1 means that the window is scrolled completely to the right</param>
        public void Scroll(double factor)
        {
            double scrollSize = this.WindowDuration * factor;
            this.MinShownTime += scrollSize;
            this.MaxShownTime += scrollSize;
            this.OnZoom();
        }

        /// <summary>
        /// Scrolls the window in order to place the cursor in the center of the window.
        /// </summary>
        public void CenterCursor()
        {
            double beforeCursor = this.CursorTime - this.MinShownTime;
            double afterCursor = this.MaxShownTime - this.CursorTime;
            double delta = (beforeCursor - afterCursor) / 2;

            this.MinShownTime += delta;
            this.MaxShownTime += delta;
            this.PerformZoom(0);
        }

        /// <summary>
        /// This method is used to store the changed size of the main window to the registry.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                // The new size is stored only if the window is in Normal mode
                GuiUtil.SaveMainWindowPositionInRegistry(this.Left, this.Top, this.ActualWidth, this.ActualHeight);
            }
        }

        /// <summary>
        /// This method is used to store the changed location of the main window to the registry.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            GuiUtil.SaveMainWindowPositionInRegistry(this.Left, this.Top, this.ActualWidth, this.ActualHeight);
        }

        /// <summary>
        /// This method is used to show or hide the lateral panel with the configuration and protocol views
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        /*private void SideVisible_Click(object sender, RoutedEventArgs e)
        {
            this.SideView.Visibility =
                (this.SideVisible.IsChecked == true) ?
                Visibility.Visible :
                Visibility.Collapsed;
        }*/
    }
}