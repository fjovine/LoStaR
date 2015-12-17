//-----------------------------------------------------------------------
// <copyright file="CursorOverlay.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System.Collections.Generic;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Overlay canvas that is used to draw the main cursor and the auxiliary cursors.
    /// The main cursor is normally modifiable directly with the cursor.
    /// The auxiliary cursor are normally less dynamic than the main cursor
    /// and show some computed information on the stripes.
    /// </summary>
    public class CursorOverlay : Stripe
    {
        /// <summary>
        /// List of the auxiliary cursors.
        /// </summary>
        private List<AuxiliaryCursor> auxiliaryCursors = new List<AuxiliaryCursor>();

        /// <summary>
        /// Gets or sets the position of the cursor in seconds.
        /// </summary>
        public double CursorPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Adds an auxiliary cursor.
        /// </summary>
        /// <param name="position">Position of the cursor in seconds.</param>
        /// <param name="brush">Brush to be used to render it.</param>
        /// <returns>The auxiliary cursor added.</returns>
        public AuxiliaryCursor AddAuxiliaryCursor(double position, Brush brush)
        {
            AuxiliaryCursor result = new AuxiliaryCursor()
            {
                Position = position,
                CursorBrush = brush
            };
            this.auxiliaryCursors.Add(result);
            return result;
        }

        /// <summary>
        /// Clears all the auxiliary cursors.
        /// </summary>
        public void ClearAuxiliaryCursors()
        {
            this.auxiliaryCursors.Clear();
        }

        /// <summary>
        /// Redraws the main cursor and the auxiliary cursors
        /// </summary>
        public override void Redraw()
        {
            double scaledPosition = this.ScaleX(this.CursorPosition);
            this.Children.Add(
                new Line()
                {
                    X1 = scaledPosition,
                    X2 = scaledPosition,
                    Y1 = 0,
                    Y2 = this.ActualHeight,
                    Stroke = Brushes.Red,
                    StrokeThickness = 0.7
                });
            foreach (var cursor in this.auxiliaryCursors)
            {
                scaledPosition = this.ScaleX(cursor.Position);
                this.Children.Add(
                    new Line()
                    {
                        X1 = scaledPosition,
                        X2 = scaledPosition,
                        Y1 = 0,
                        Y2 = this.ActualHeight,
                        Stroke = cursor.CursorBrush,
                        StrokeThickness = 0.7
                    });
            }
        }

        /// <summary>
        /// Utility class to describe auxiliary cursors that are not moved by the cursor
        /// </summary>
        public class AuxiliaryCursor
        {
            /// <summary>
            /// Gets or sets the position of the auxiliary cursor in seconds
            /// </summary>
            public double Position
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the brush to be used for the auxiliary cursors
            /// </summary>
            public Brush CursorBrush
            {
                get;
                set;
            }
        }
    }
}
