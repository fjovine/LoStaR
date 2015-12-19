//-----------------------------------------------------------------------
// <copyright file="XamlToUIElementConverter.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Media;

    /// <summary>
    /// Converter class used to transform <c>svg -> xaml</c> transformed graphics into images
    /// </summary>
    public class XamlToUIElementConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <c>xaml</c> into an image.
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">Filename of the <c>xaml</c> to transform.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>The transformed image.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LoStar.Resources." + (string)parameter))
            {
                return XamlReader.Load(stream) as Viewbox;
            }
        }

        /// <summary>
        /// Method not implemented.
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>Never returns.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}