//-----------------------------------------------------------------------
// <copyright file="FixedSpaceTxtDocument.cs" company="hiLab">
//     Copyright (c) Francesco Iovine.
// </copyright>
// <author>Francesco Iovine iovinemeccanica@gmail.com</author>
//-----------------------------------------------------------------------
namespace LoStar
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Class to build a table in a text file.
    /// Such table is composed by fixed width columns that can be left or right aligned.
    /// </summary>
    public class FixedSpaceTxtDocument
    {
        /// <summary>
        /// String builder where the output line is built.
        /// </summary>
        private StringBuilder line;

        /// <summary>
        /// Title of the document.
        /// </summary>
        private string title;

        /// <summary>
        /// Index of the current field being input.
        /// </summary>
        private int currentField;

        /// <summary>
        /// This object is called sequentially for every field.
        /// The first parameter is the line number, the second parameter is the field index.
        /// Both start from 0.
        /// Returns the field, null if the file is finished.
        /// The if the string starts with [, the field is left aligned, if it starts with ] the field is right aligned.
        /// If it is string.empty the field is void
        /// </summary>
        private Func<int, int, string> getField;

        /// <summary>
        /// Array containing the column widths in characters.
        /// </summary>
        private int[] columnWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSpaceTxtDocument" /> class.
        /// </summary>
        /// <param name="title">Title to be written on top of the document.</param>
        /// <param name="columnWidth">Integer array containing the width of each column</param>
        /// <param name="getField"><c>Func</c> receiving the line and column index of the field as parameters, returning a string with the field content.
        /// When the string is null, the file is finished. The first string character is a special formatting character.</param>
        public FixedSpaceTxtDocument(string title, int[] columnWidth, Func<int, int, string> getField)
        {
            int lineWidth = columnWidth.Sum();
            this.line = new StringBuilder(lineWidth);
            this.getField = getField;
            this.columnWidth = columnWidth;
            this.title = title;
        }

        /// <summary>
        /// Prints out all the content of the FixedSpaceTxtDocument.
        /// </summary>
        /// <param name="tw">TextWriter to be used.</param>
        public void PrintAll(TextWriter tw)
        {
            int lineNo = 0;

            tw.WriteLine(this.title);
            while (true)
            {
                this.currentField = 0;
                for (int col = 0; col < this.columnWidth.Length; col++)
                {
                    string toPrint = this.getField(lineNo, col);
                    if (toPrint == null)
                    {
                        return;
                    }

                    if (toPrint == string.Empty)
                    {
                        toPrint = " ";
                    }

                    if (toPrint[0] == '\n')
                    {
                        // If the line terminates too early, it finishes the line
                        while (col < this.columnWidth.Length)
                        {
                            this.WriteField(" ");
                            col++;
                        }

                        break;
                    }

                    if (toPrint[0] == 'b')
                    {
                        // Leaves a void line
                        break;
                    }

                    this.WriteField(toPrint.Substring(1), toPrint[0] == 'r');
                }

                tw.WriteLine(this.line);
                this.line.Clear();
                lineNo++;
            }
        }

        /// <summary>
        /// Writes the current field.
        /// </summary>
        /// <param name="what">String to be written.</param>
        /// <param name="isRightAligned">True if the field is right aligned.</param>
        private void WriteField(string what, bool isRightAligned = false)
        {
            int strLen = what.Length;
            int colWid = this.columnWidth[this.currentField++];
            if (strLen > colWid)
            {
                if (colWid > 3)
                {
                    this.line.Append(what.Substring(0, colWid - 3));
                    this.line.Append("...");
                }
                else
                {
                    for (int i = 0; i < colWid; i++)
                    {
                        this.line.Append(' ');
                    }
                }
            }
            else
            {
                if (isRightAligned)
                {
                    for (int i = 0; i < colWid - strLen; i++)
                    {
                        this.line.Append(' ');
                    }

                    this.line.Append(what);
                }
                else
                {
                    this.line.Append(what);
                    for (int i = 0; i < colWid - strLen; i++)
                    {
                        this.line.Append(' ');
                    }
                }
            }

            this.line.Append('|');
        }
    }
}
