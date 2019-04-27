using System;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace MainGUI
{
    public static class PopUpExtensions
    {
        /// <summary>
        /// Show Pop up windows for x seconds.
        /// </summary>
        /// <param name="popup">the pop up window to show</param>
        /// <param name="seconds"></param>
        public static void Show(this Popup popup, uint seconds = 5)
        {
            popup.IsOpen = true;

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(seconds)
            };

            timer.Tick += delegate
            {
                timer.Stop();
                if (popup.IsOpen)
                    popup.IsOpen = false;
            };

            timer.Start();
        }
    }
}
