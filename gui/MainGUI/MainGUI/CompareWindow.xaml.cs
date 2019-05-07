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

            PricePercentageLabel.Content = NpePercentageLabel.Content = TotalCostsPercentageLabel.Content = "";

            EconomicsTab.SelectedIndex = 0;
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

            switch (EconomicsTab.SelectedIndex)
            {
                // simple economics
                case 0:
                    CompareSimpleEconomics(otherSysPe);
                    break;
                case 1:
                    CompareAdvancedEconomics();
                    break;

                default:
                    throw new NotImplementedException("Economics option not implemented.");
            }


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
            SetColors(SystemCalculation.GetSysPe, otherSysPe, ThisSystemPeResultTextbox, OtherSystemPeResultTextbox,
                PePercentageLabel, true);

            // Heat capacity
            SetColors(SystemCalculation.GetSysPhe, otherSysPhe, ThisSystemPheResultTextbox, OtherSystemPheResultTextbox,
                PhePercentageLabel);
        }

        private void CompareCOP(double otherSysCOP)
        {
            SetColors(SystemCalculation.GetSysCOP, otherSysCOP, ThisSystemCOPResultTextbox, OtherSystemCOPResultTextbox,
                COPPercentageLabel);
        }

        private void CompareSimpleEconomics(double otherSysPe)
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

            SetColors(thisSysPrice, otherSysPrice, ThisSystemElectricCostsTextBox, OtherSystemElectricCostsTextBox,
                PricePercentageLabel, true);
        }

        private void CompareAdvancedEconomics()
        {
            if (!GUIChecks.TryGetValue(CeTextBox, out double Ce, false) ||
                !GUIChecks.TryGetValue(CqTextBox, out double Cq, false) ||
                !GUIChecks.TryGetValue(QrTextBox, out double Qr, false) ||
                !GUIChecks.TryGetValue(EfficiencyTextBox, out double efficiency, false) ||
                !GUIChecks.TryGetValue(ThisSystemNprTextBox, out double thisSysNpr, false) ||
                !GUIChecks.TryGetValue(ThisSystemINTextBox, out double thisSysIN, false) ||
                !GUIChecks.TryGetValue(ThisSystemJINTextBox, out double thisSysJIN, false) ||
                !GUIChecks.TryGetValue(OtherSystemNprTextBox, out double otherSysNpr, false) ||
                !GUIChecks.TryGetValue(OtherSystemINTextBox, out double otherSysIN, false) ||
                !GUIChecks.TryGetValue(OtherSystemJINTextBox, out double otherSysJIN, false))
            {
                NpePercentageLabel.Content = TotalCostsPercentageLabel.Content = "";
                ThisSystemNpeResultTextBox.Background = OtherSystemNpeResultTextBox.Background = Brushes.White;
                ThisSystemTotalCostsTextBox.Background = OtherSystemTotalCostsTextBox.Background = Brushes.White;
                return;
            }

            // Npe
            double thisSysNpe = Ce * Qr * (1 / SystemCalculation.GetSysCOP);
            double otherSysNpe = Cq * Qr * (1 / efficiency);

            SetColors(thisSysNpe, otherSysNpe, ThisSystemNpeResultTextBox, OtherSystemNpeResultTextBox,
                NpePercentageLabel, true);

            ThisSystemNpeResultTextBox.Text = thisSysNpe.ToString("F1");
            OtherSystemNpeResultTextBox.Text = otherSysNpe.ToString("F1");

            // total costs
            double thisSysTotalCosts = thisSysNpe + thisSysIN * thisSysJIN + thisSysNpr;
            double otherSysTotalCosts = otherSysNpe + otherSysIN * otherSysJIN + otherSysNpr;

            SetColors(thisSysTotalCosts, otherSysTotalCosts, ThisSystemTotalCostsTextBox, OtherSystemTotalCostsTextBox,
                TotalCostsPercentageLabel, true);

            ThisSystemTotalCostsTextBox.Text = thisSysTotalCosts.ToString("F1");
            OtherSystemTotalCostsTextBox.Text = otherSysTotalCosts.ToString("F1");
        }

        private void SetColors(double thisSysValue, double otherSysValue, TextBox thisSysTextBox,
            TextBox otherSysTextBox, Label percentageLabel, bool invertedColors = false)
        {
            if (Math.Abs(thisSysValue - otherSysValue) < 0.001)
            {
                percentageLabel.Content = "=";
                thisSysTextBox.Background = otherSysTextBox.Background = Brushes.White;
            }
            else if (thisSysValue > otherSysValue)
            {
                if (invertedColors)
                {
                    thisSysTextBox.Background = Brushes.Red;
                    otherSysTextBox.Background = Brushes.Green;
                }
                else
                {
                    thisSysTextBox.Background = Brushes.Green;
                    otherSysTextBox.Background = Brushes.Red;
                }

                percentageLabel.Content = "> " + Math.Round((thisSysValue / otherSysValue - 1) * 100, 3) + " %";
            }
            else if (thisSysValue < otherSysValue)
            {
                if (invertedColors)
                {
                    thisSysTextBox.Background = Brushes.Green;
                    otherSysTextBox.Background = Brushes.Red;
                }
                else
                {
                    thisSysTextBox.Background = Brushes.Red;
                    otherSysTextBox.Background = Brushes.Green;
                }

                percentageLabel.Content = "< " + Math.Round((otherSysValue / thisSysValue - 1) * 100, 3) + " %";
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

        private void EconomicsTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (EconomicsTab.SelectedIndex)
            {
                // Simple
                case 0:
                    SimpleEconomicsCanvas.Visibility = Visibility.Visible;
                    AdvancedEconomicsCanvas.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    SimpleEconomicsCanvas.Visibility = Visibility.Hidden;
                    AdvancedEconomicsCanvas.Visibility = Visibility.Visible;
                    break;

                default:
                    throw new NotImplementedException("Economics option not implemented.");
            }
        }
    }
}