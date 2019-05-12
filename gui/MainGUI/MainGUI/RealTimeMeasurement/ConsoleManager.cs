using System;
using System.Windows.Controls;

namespace MainGUI.RealTimeMeasurement
{
    public class ConsoleManager
    {
        public TextBox ConsoleTextBox;

        public ConsoleManager(TextBox console)
        {
            ConsoleTextBox = console;
        }

        public void ConsoleWriteMessage(string message)
        {
            ConsoleTextBox.Dispatcher.Invoke(() => 
                ConsoleTextBox.AppendText(DateTime.Now.ToString("hh:mm:ss") + " " + message + '\n'));
            ConsoleTextBox.Dispatcher.Invoke(() => ConsoleTextBox.ScrollToEnd());
            /* if (ConsoleTextBox.Dispatcher.CheckAccess())
             {
                 ConsoleTextBox.Text += DateTime.Now.ToString("hh:mm:ss") + " " + message + '\n';
             }
             else
             {
                 // This thread does not have access to the UI thread.
                 // Place the update method on the Dispatcher of the UI thread.
                 ConsoleTextBox.Dispatcher.Invoke(() =>
                     ConsoleTextBox.Text += DateTime.Now.ToString("hh:mm:ss") + " " + message + '\n');
             }*/
        }

        public void ClearConsole()
        {
            ConsoleTextBox.Clear();
        }
    }
}