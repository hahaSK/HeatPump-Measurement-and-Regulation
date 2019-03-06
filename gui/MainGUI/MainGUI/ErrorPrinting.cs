using System.Windows;

namespace MainGUI
{
    public static class ErrorPrinting
    {
        public static void PrintError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK);
        }

    }
}
