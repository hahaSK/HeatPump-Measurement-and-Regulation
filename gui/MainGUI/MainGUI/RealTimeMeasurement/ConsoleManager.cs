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
        }

        public void ClearConsole()
        {
            ConsoleTextBox.Clear();
        }
    }
}