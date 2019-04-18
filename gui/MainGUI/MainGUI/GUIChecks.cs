﻿using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace MainGUI
{
    public static class GUIChecks
    {
        private static readonly Regex Regex = new Regex("[^0-9.,-]+"); //regex that matches disallowed text

        /// <summary>
        /// Tries to parse the text of text box to double.
        /// </summary>
        /// <param name="textBox">Text box to parse</param>
        /// <param name="result">Result of the parse</param>
        /// <returns><see cref="T:True" /> if the parse was successful</returns>
        public static bool TryGetValue(TextBox textBox, out double result)
        {
            if (Double.TryParse(textBox.Text, out double parseResult))
            {
                result = parseResult;
                return true;
            }

            ErrorPrinting.PrintError("Couldn't parse " + textBox.Text + " of " + textBox.Name + "!");
            result = 0;
            return false;
        }

        public static bool IsTextAllowed(string text)
        {
            return !Regex.IsMatch(text);
        }

        public static void ReplaceDot(object sender, TextChangedEventArgs e)
        {
            if (!(sender is TextBox tb))
            {
                ErrorPrinting.PrintError("Replace Dot " + sender.ToString() + " not a TextBox!");
                return;
            }

            using (tb.DeclareChangeBlock())
            {
                foreach (var c in e.Changes)
                {
                    if (c.AddedLength == 0) continue;
                    tb.Select(c.Offset, c.AddedLength);
                    if (tb.SelectedText.Contains("."))
                    {
                        tb.SelectedText = tb.SelectedText.Replace('.', ',');
                    }

                    tb.Select(c.Offset + c.AddedLength, 0);
                }
            }
        }
    }
}