using System.Windows.Controls;

namespace MainGUI
{
    public class GUIChecks
    {
        /// <summary>
        /// Tries to parse the text of text box to double.
        /// </summary>
        /// <param name="textBox">Text box to parse</param>
        /// <param name="result">Result of the parse</param>
        /// <returns><see cref="T:True" /> if the parse was successful</returns>
        public bool TryGetValue(TextBox textBox, out double result)
        {
            if (double.TryParse(textBox.Text, out double parseResult))
            {
                result = parseResult;
                return true;
            }

            ErrorPrinting.PrintError("Couldn't parse " + textBox.Text + " of " + textBox.Name + "!");
            result = 0;
            return false;
        }
    }
}