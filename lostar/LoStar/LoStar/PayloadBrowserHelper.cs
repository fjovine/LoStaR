//-----------------------------------------------------------------------
// <copyright file="PayloadBrowserHelper.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Helper class to show the payload in a table oriented manner.
    /// </summary>
    public class PayloadBrowserHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayloadBrowserHelper" /> class.
        /// </summary>
        /// <param name="address">Starting address of the payload row to be managed</param>
        /// <param name="content">Content of the payload</param>
        /// <param name="bytePerRow">Count of the bytes to be shown per row.</param>
        public PayloadBrowserHelper(int address, List<byte> content, int bytePerRow = 8)
        {
            if (address > content.Count)
            {
                throw new ArgumentException("The address is outside the range");
            }

            StringBuilder hex = new StringBuilder((3 * bytePerRow) - 1);
            StringBuilder ascii = new StringBuilder(bytePerRow);
            for (int i = address; i < address + bytePerRow; i++)
            {
                if (i < content.Count)
                {
                    byte current = content[i];
                    if (i > address)
                    {
                        hex.Append(' ');
                    }

                    hex.Append(current.ToHex());
                    ascii.Append(current.PrintPrintableOrPoint());
                }
            }

            this.Address = ((byte)address).ToHex();
            this.Hex = hex.ToString();
            this.Ascii = ascii.ToString();
        }

        /// <summary>
        /// Gets the address of the first byte of the row.
        /// </summary>
        public string Address
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Hex string interpreting the payload row
        /// </summary>
        public string Hex
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <c>Ascii</c> string (with dot for unprintable bytes) interpreting the payload row
        /// </summary>
        public string Ascii
        {
            get;
            private set;
        }
    }
}
