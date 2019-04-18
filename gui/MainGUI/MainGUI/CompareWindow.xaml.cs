using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MainGUI
{
    /// <summary>
    /// Interaction logic for CompareWindow.xaml
    /// </summary>
    public partial class CompareWindow : Window
    {
       private readonly string[] _otherSystemInputOptions = {"COP", "Phe and Pe"};

        public CompareWindow(double COP, double Phe, double Pe)
        {
            InitializeComponent();

            ThisSystemCOPTextbox.Text = COP.ToString("F1");
            ThisSystemPheTextbox.Text = Phe.ToString("F1");
            ThisSystemPeTextbox.Text = Pe.ToString("F1");

            OtherSystemInputOptions.ItemsSource = _otherSystemInputOptions;

            ThisSystemResultCanvas.Visibility = Visibility.Hidden;
            OtherSystemResultCanvas.Visibility = Visibility.Hidden;

            COPPePercentageLabel.Visibility = Visibility.Hidden;
            PhePercentageLabel.Visibility = Visibility.Hidden;
        }

        private void OtherSystemInputOptions_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (OtherSystemInputOptions.SelectedIndex)
            {
                case 0:
                    OtherSysCOPCanvas.Visibility = Visibility.Visible;
                    OtherSysPePheCanvas.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    OtherSysCOPCanvas.Visibility = Visibility.Hidden;
                    OtherSysPePheCanvas.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Compare())
                return;
            
            SetResultVisibility();
        }

        private bool Compare()
        {
            switch (OtherSystemInputOptions.SelectedIndex)
            {
                case 0:
                    if (!GUIChecks.TryGetValue(OtherSystemCOPTextbox, out double otherSysCOP)) return false;
                    CompareCOP(otherSysCOP);
                    break;
                case 1:
                    if (!GUIChecks.TryGetValue(OtherSystemPeTextbox, out double otherSysPe) ||
                        !GUIChecks.TryGetValue(OtherSystemPheTextbox, out double otherSysPhe)) return false;
                    CompareCapacities(otherSysPhe, otherSysPe);
                    break;
            }

            return true;
        }

        private void CompareCapacities(double otherSysPhe, double otherSysPe)
        {
            ThisSystemPeResultTextbox.Text = ThisSystemPeTextbox.Text;
            ThisSystemPheResultTextbox.Text = ThisSystemPheTextbox.Text;

            OtherSystemPeResultTextbox.Text = OtherSystemPeTextbox.Text;
            OtherSystemPheResultTextbox.Text = OtherSystemPheTextbox.Text;
            // TODO move to another method

            // Electrical consumption [W]

            if (Math.Abs(SystemCalculation.GetSysPe - otherSysPe) < 0.001)
            {
                COPPePercentageLabel.Content = "=";
            }
            else if (SystemCalculation.GetSysPe > otherSysPe)
            {
                ThisSystemPeResultTextbox.Background = Brushes.Green;
                OtherSystemPeResultTextbox.Background = Brushes.DarkRed;
                COPPePercentageLabel.Content = "> " + Math.Round((SystemCalculation.GetSysPe / otherSysPe - 1) * 100, 3) + " %";
            }
            else if (SystemCalculation.GetSysPe < otherSysPe)
            {
                ThisSystemPeResultTextbox.Background = Brushes.DarkRed;
                OtherSystemPeResultTextbox.Background = Brushes.Green;
                COPPePercentageLabel.Content = "< " + Math.Round((otherSysPe / SystemCalculation.GetSysPe - 1) * 100, 3) + " %";
            }

            // Heat capacity

            if (Math.Abs(SystemCalculation.GetSysPhe - otherSysPhe) < 0.001)
            {
                PhePercentageLabel.Content = "=";
            }
            else if (SystemCalculation.GetSysPhe > otherSysPhe)
            {
                ThisSystemPheResultTextbox.Background = Brushes.Green;
                OtherSystemPheResultTextbox.Background = Brushes.DarkRed;
                PhePercentageLabel.Content = "> " + Math.Round((SystemCalculation.GetSysPhe / otherSysPhe - 1) * 100, 3) + " %";
            }
            else if (SystemCalculation.GetSysPhe < otherSysPhe)
            {
                ThisSystemPheResultTextbox.Background = Brushes.DarkRed;
                OtherSystemPheResultTextbox.Background = Brushes.Green;
                PhePercentageLabel.Content = "< " + Math.Round((otherSysPhe / SystemCalculation.GetSysPhe - 1) * 100, 3) + " %";
            }
        }

        private void CompareCOP(double otherSysCOP)
        {
            ThisSystemCOPResultTextbox.Text = ThisSystemCOPTextbox.Text;
            OtherSystemCOPResultTextbox.Text = OtherSystemCOPTextbox.Text;

            if (Math.Abs(SystemCalculation.GetSysCOP - otherSysCOP) < 0.001)
            {
                COPPePercentageLabel.Content = "=";
            }
            else if (SystemCalculation.GetSysCOP > otherSysCOP)
            {
                ThisSystemCOPResultTextbox.Background = Brushes.Green;
                OtherSystemCOPResultTextbox.Background = Brushes.DarkRed;
                COPPePercentageLabel.Content = "> " + Math.Round((SystemCalculation.GetSysCOP / otherSysCOP - 1) * 100, 3) + " %";
            }
            else if (SystemCalculation.GetSysCOP < otherSysCOP)
            {
                ThisSystemCOPResultTextbox.Background = Brushes.DarkRed;
                OtherSystemCOPResultTextbox.Background = Brushes.Green;
                COPPePercentageLabel.Content = "< " + Math.Round((otherSysCOP / SystemCalculation.GetSysCOP - 1) * 100, 3) + " %";
            }
        }

        private void SetResultVisibility()
        {
            ThisSystemResultCanvas.Visibility = Visibility.Visible;
            OtherSystemResultCanvas.Visibility = Visibility.Visible;

            // The COP and Pe share the same label so they will be always visible
            COPPePercentageLabel.Visibility = Visibility.Visible;

            switch (OtherSystemInputOptions.SelectedIndex)
            {
                case 0:
                    ThisSystemCOPResultCanvas.Visibility = Visibility.Visible;
                    OtherSystemCOPResultCanvas.Visibility = Visibility.Visible;
                    ThisSystemPePheResultCanvas.Visibility = Visibility.Hidden;
                    OtherSystemPePheResultCanvas.Visibility = Visibility.Hidden;

                    PhePercentageLabel.Visibility = Visibility.Hidden;
                    break;

                case 1:
                    ThisSystemCOPResultCanvas.Visibility = Visibility.Hidden;
                    OtherSystemCOPResultCanvas.Visibility = Visibility.Hidden;
                    ThisSystemPePheResultCanvas.Visibility = Visibility.Visible;
                    OtherSystemPePheResultCanvas.Visibility = Visibility.Visible;

                    PhePercentageLabel.Visibility = Visibility.Visible;
                    break;
            }
        }

        #region TextChangeTextBoxes

        private void OtherSystemCOPTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            GUIChecks.ReplaceDot(sender, e);
        }

        private void OtherSystemPeTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            GUIChecks.ReplaceDot(sender, e);
        }

        private void OtherSystemPheTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            GUIChecks.ReplaceDot(sender, e);
        }

        #endregion
    }
}