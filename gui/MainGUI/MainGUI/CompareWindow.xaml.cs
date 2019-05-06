using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MainGUI
{
    /// <summary>
    /// Interaction logic for CompareWindow.xaml
    /// </summary>
    public partial class CompareWindow : Window
    {
        public CompareWindow(double COP, double Phe, double Pe)
        {
            InitializeComponent();

            ThisSystemCOPTextbox.Text = COP.ToString("F1");
            ThisSystemPheTextbox.Text = Phe.ToString("F1");
            ThisSystemPeTextbox.Text = Pe.ToString("F1");

            ThisSystemResultCanvas.Visibility = Visibility.Visible;
            OtherSystemResultCanvas.Visibility = Visibility.Visible;

            COPPercentageLabel.Visibility = Visibility.Hidden;
            PePercentageLabel.Visibility = Visibility.Hidden;
            PhePercentageLabel.Visibility = Visibility.Hidden;

            PricePercentageLabel.Content = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Compare())
                return;

            SetResultVisibility();
        }

        private bool Compare()
        {
            if (!GUIChecks.TryGetValue(OtherSystemPeTextbox, out double otherSysPe) ||
                !GUIChecks.TryGetValue(OtherSystemPheTextbox, out double otherSysPhe)) return false;

            double otherSysCOP = Math.Round(otherSysPhe / otherSysPe, 1);
            ResultTextBoxes(otherSysCOP);
            CompareCapacities(otherSysPhe, otherSysPe);
            CompareCOP(otherSysCOP);

            CompareEconomics(otherSysPe);

            return true;
        }

        private void ResultTextBoxes(double otherSysCOP)
        {
            ThisSystemPeResultTextbox.Text = ThisSystemPeTextbox.Text;
            ThisSystemPheResultTextbox.Text = ThisSystemPheTextbox.Text;
            ThisSystemCOPResultTextbox.Text = ThisSystemCOPTextbox.Text;

            OtherSystemPeResultTextbox.Text = OtherSystemPeTextbox.Text;
            OtherSystemPheResultTextbox.Text = OtherSystemPheTextbox.Text;
            OtherSystemCOPResultTextbox.Text = otherSysCOP.ToString("F1");
        }

        private void CompareCapacities(double otherSysPhe, double otherSysPe)
        {
            // Electrical consumption [W]

            if (Math.Abs(SystemCalculation.GetSysPe - otherSysPe) < 0.001)
            {
                PePercentageLabel.Content = "=";
                ThisSystemPeResultTextbox.Background = OtherSystemPeResultTextbox.Background = Brushes.White;
            }
            else if (SystemCalculation.GetSysPe > otherSysPe)
            {
                ThisSystemPeResultTextbox.Background = Brushes.Red;
                OtherSystemPeResultTextbox.Background = Brushes.Green;
                PePercentageLabel.Content =
                    "> " + Math.Round((SystemCalculation.GetSysPe / otherSysPe - 1) * 100, 3) + " %";
            }
            else if (SystemCalculation.GetSysPe < otherSysPe)
            {
                ThisSystemPeResultTextbox.Background = Brushes.Green;
                OtherSystemPeResultTextbox.Background = Brushes.Red;
                PePercentageLabel.Content =
                    "< " + Math.Round((otherSysPe / SystemCalculation.GetSysPe - 1) * 100, 3) + " %";
            }

            // Heat capacity

            if (Math.Abs(SystemCalculation.GetSysPhe - otherSysPhe) < 0.001)
            {
                PhePercentageLabel.Content = "=";
                ThisSystemPheResultTextbox.Background = OtherSystemPheResultTextbox.Background = Brushes.White;
            }
            else if (SystemCalculation.GetSysPhe > otherSysPhe)
            {
                ThisSystemPheResultTextbox.Background = Brushes.Green;
                OtherSystemPheResultTextbox.Background = Brushes.Red;
                PhePercentageLabel.Content =
                    "> " + Math.Round((SystemCalculation.GetSysPhe / otherSysPhe - 1) * 100, 3) + " %";
            }
            else if (SystemCalculation.GetSysPhe < otherSysPhe)
            {
                ThisSystemPheResultTextbox.Background = Brushes.Red;
                OtherSystemPheResultTextbox.Background = Brushes.Green;
                PhePercentageLabel.Content =
                    "< " + Math.Round((otherSysPhe / SystemCalculation.GetSysPhe - 1) * 100, 3) + " %";
            }
        }

        private void CompareCOP(double otherSysCOP)
        {
            if (Math.Abs(SystemCalculation.GetSysCOP - otherSysCOP) < 0.001)
            {
                COPPercentageLabel.Content = "=";
                ThisSystemCOPResultTextbox.Background = OtherSystemCOPResultTextbox.Background = Brushes.White;
            }
            else if (SystemCalculation.GetSysCOP > otherSysCOP)
            {
                ThisSystemCOPResultTextbox.Background = Brushes.Green;
                OtherSystemCOPResultTextbox.Background = Brushes.Red;
                COPPercentageLabel.Content =
                    "> " + Math.Round((SystemCalculation.GetSysCOP / otherSysCOP - 1) * 100, 3) + " %";
            }
            else if (SystemCalculation.GetSysCOP < otherSysCOP)
            {
                ThisSystemCOPResultTextbox.Background = Brushes.Red;
                OtherSystemCOPResultTextbox.Background = Brushes.Green;
                COPPercentageLabel.Content =
                    "< " + Math.Round((otherSysCOP / SystemCalculation.GetSysCOP - 1) * 100, 3) + " %";
            }
        }

        private void CompareEconomics(double otherSysPe)
        {
            if (!GUIChecks.TryGetValue(PriceForkWhTextBox, out double priceForkWh, false))
            {
                PricePercentageLabel.Content = "";
                ThisSystemElectricCostsTextBox.Background = OtherSystemElectricCostsTextBox.Background = Brushes.White;
                return;
            }

            double thisSysPrice = priceForkWh * (SystemCalculation.GetSysPe / 1000);
            double otherSysPrice = priceForkWh * (otherSysPe / 1000);

            // copy to text boxes
            ThisSystemElectricCostsTextBox.Text = thisSysPrice.ToString("F1");
            OtherSystemElectricCostsTextBox.Text = otherSysPrice.ToString("F1");

            if (Math.Abs(thisSysPrice - otherSysPrice) < 0.001)
            {
                PricePercentageLabel.Content = "=";
                ThisSystemElectricCostsTextBox.Background = OtherSystemElectricCostsTextBox.Background = Brushes.White;
            }
            else if (thisSysPrice > otherSysPrice)
            {
                ThisSystemElectricCostsTextBox.Background = Brushes.Red;
                OtherSystemElectricCostsTextBox.Background = Brushes.Green;
                PricePercentageLabel.Content = "> " + Math.Round((thisSysPrice / otherSysPrice - 1) * 100, 3) + " %";
            }
            else if (thisSysPrice < otherSysPrice)
            {
                ThisSystemElectricCostsTextBox.Background = Brushes.Green;
                OtherSystemElectricCostsTextBox.Background = Brushes.Red;
                PricePercentageLabel.Content = "< " + Math.Round((otherSysPrice / thisSysPrice - 1) * 100, 3) + " %";
            }
        }

        private void SetResultVisibility()
        {
            PePercentageLabel.Visibility = Visibility.Visible;
            PhePercentageLabel.Visibility = Visibility.Visible;
            COPPercentageLabel.Visibility = Visibility.Visible;
        }

        #region Text Box events

        private void OnTextBoxChange(object sender, TextChangedEventArgs e)
        {
            GUIChecks.ReplaceDot(sender, e);
        }

        private void TextBoxKeyDownPreview(object sender, KeyEventArgs e)
        {
            e.Handled = !GUIChecks.IsSpaceAllowed(e);
        }

        private void TextBoxTextPreviewNonNegative(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !GUIChecks.IsTextAllowedForDoubleNonNegative(e.Text);
        }

        #endregion
    }
}