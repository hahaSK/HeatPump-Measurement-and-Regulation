using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace MainGUI
{
    /// <summary>
    /// Interaction logic for BivalentSimulation.xaml
    /// </summary>
    public partial class BivalentSimulation : Window
    {
        public enum Mode
        {
            Monovalent,
            AlternativeBivalent,
            ParallelBivalent,
            PartiallyBivalent
        }

        private readonly string[] _modeNames =
            {"Monovalent", "Alternative bivalent", "Parallel bivalent", "Partially bivalent"};

        private readonly BivalentGraph _bivalentGraph = new BivalentGraph();
        private readonly BivalentUtilization _bivalentUtilization = new BivalentUtilization();
        private Mode _mode = Mode.Monovalent;

        private readonly bool _isInitializing;

        public BivalentSimulation()
        {
            _isInitializing = true;
            InitializeComponent();

            _bivalentGraph.heatLossChanged += OnHeatLossChanged;

            ModeCombBox.ItemsSource = _modeNames;

            SetModeSchema();

            BivalentPointChart.LegendLocation = LegendLocation.Left;
            // always will be 0
            Axis myYAxis = new Axis
            {
                MinValue = 0,
                Title = "Heat Loss [W]",
                Foreground = Brushes.Black,
                Separator = new LiveCharts.Wpf.Separator
                    {Stroke = Brushes.Gray, StrokeDashArray = new DoubleCollection {4, 2}}
            };
            BivalentPointChart.AxisY.Clear();
            BivalentPointChart.AxisY.Add(myYAxis);
            // default x axis
            Axis myXAxis = new Axis
            {
                MinValue = -20,
                MaxValue = 22,
                Title = "Temperature [°C]",
                Foreground = Brushes.Black,
                Separator = new LiveCharts.Wpf.Separator
                    {Step = 2, Stroke = Brushes.Gray, StrokeDashArray = new DoubleCollection {4, 2}}
            };
            BivalentPointChart.AxisX.Clear();
            BivalentPointChart.AxisX.Add(myXAxis);

            SetSliderMaximum();

            // graph is redrawn by this change
            ModeCombBox.SelectedIndex = 0;

            _bivalentGraph.DrawCurrentHeatLoss(BivalentPointChart, CurrentTempSlider.Value);

            _isInitializing = false;
        }

        private void RedrawGraph()
        {
            if (!GUIChecks.TryGetValue(AmbientMaxTEmpTxtBox, out _bivalentGraph.AmbientMaxTemp) ||
                !GUIChecks.TryGetValue(AmbientMinTempTxtBox, out _bivalentGraph.AmbientMinTemp) ||
                !GUIChecks.TryGetValue(ObjectHeatLossCoeffTxtBox, out _bivalentGraph.HeatLossCoefficient) ||
                !GUIChecks.TryGetValue(ZeroLoadTempTxtBox, out _bivalentGraph.ZeroLoadTemp)) return;
            if (_mode != Mode.Monovalent &&
                !GUIChecks.TryGetValue(BivalentTempTxtBox, out _bivalentGraph.BivalentTemp)) return;
            if (_mode == Mode.PartiallyBivalent &&
                !GUIChecks.TryGetValue(OnlyAdditionalSourceTempTextBox,
                    out _bivalentGraph.AdditionalSourceOnlyTemp)) return;

            _bivalentGraph.RedrawGraph(BivalentPointChart, _mode);
            _bivalentGraph.DrawCurrentHeatLoss(BivalentPointChart, CurrentTempSlider.Value);
        }

        /// <summary>
        /// Set schema according to mode
        /// </summary>
        private void SetModeSchema()
        {
            if (_mode == Mode.Monovalent)
            {
                MonovalentSchemaCanvas.Visibility = Visibility.Visible;
                BivalentSchemaCanvas.Visibility = Visibility.Hidden;
                BivalentTempCanvas.Visibility = Visibility.Hidden;
                AdditionalHeatSourceMaxCapacityCanvas.Visibility = Visibility.Hidden;
                AdditionalHeatSourceUtilizationCanvas.Visibility = Visibility.Hidden;
                OnlyAdditionaSourceTempCanvas.Visibility = Visibility.Hidden;
            }
            else if (_mode != Mode.Monovalent)
            {
                MonovalentSchemaCanvas.Visibility = Visibility.Hidden;
                BivalentSchemaCanvas.Visibility = Visibility.Visible;
                BivalentTempCanvas.Visibility = Visibility.Visible;
                AdditionalHeatSourceMaxCapacityCanvas.Visibility = Visibility.Visible;
                AdditionalHeatSourceUtilizationCanvas.Visibility = Visibility.Visible;
                OnlyAdditionaSourceTempCanvas.Visibility =
                    _mode == Mode.PartiallyBivalent ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
                throw new NotImplementedException("Mode not implemented");
            }
        }

        private void ModeCombBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ModeCombBox.SelectedIndex)
            {
                case 0:
                    _mode = Mode.Monovalent;
                    break;
                case 1:
                    _mode = Mode.AlternativeBivalent;
                    break;
                case 2:
                    _mode = Mode.ParallelBivalent;
                    break;
                case 3:
                    _mode = Mode.PartiallyBivalent;
                    break;

                default:
                    throw new NotImplementedException("ModeComboBox mode not implemented!");
            }

            SetModeSchema();
            RedrawGraph();

            _bivalentGraph.DrawCurrentHeatLoss(BivalentPointChart, CurrentTempSlider.Value);
        }

        private void TextBoxOnChange(object sender, TextChangedEventArgs e)
        {
            GUIChecks.ReplaceDot(sender, e);
        }

        private void TextBoxTextPreview(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !GUIChecks.IsTextAllowedForDouble(e.Text);
        }

        private void TextBoxKeyDownPreview(object sender, KeyEventArgs e)
        {
            e.Handled = !GUIChecks.IsSpaceAllowed(e);
        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == String.Empty)
                textBox.Text = "0";

            //TODO check textboxes if they are empty
            RedrawGraph();
            SetSliderMaximum();
            if (_mode == Mode.Monovalent && textBox != null && textBox.Name == AmbientMinTempTxtBox.Name)
                ShowSuggestedHeatPumpCapacity();
        }

        private void TextBoxPopUpSuggestion(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text == String.Empty)
                textBox.Text = "0";

            RedrawGraph();
            ShowSuggestedHeatPumpCapacity();
            if (!GUIChecks.TryGetValue(CurrentTempTextBox, out double txtBoxValue))
                _bivalentGraph.DrawCurrentHeatLoss(BivalentPointChart, txtBoxValue);

            CalculateUtilization();
        }

        private void ShowSuggestedHeatPumpCapacity()
        {
            var child = SuggestedHeatPumpCapacityPopUp.Child;
            if (!(child is TextBlock textBlock))
                return;

            double sugCapacity;
            switch (_mode)
            {
                case Mode.Monovalent: //TODO check if MAX > MIN - make it only once !
                    sugCapacity = (_bivalentGraph.ZeroLoadTemp - _bivalentGraph.AmbientMinTemp) *
                                  _bivalentGraph.HeatLossCoefficient;
                    break;
                case Mode.AlternativeBivalent:
                case Mode.ParallelBivalent:
                    sugCapacity = (_bivalentGraph.ZeroLoadTemp - _bivalentGraph.BivalentTemp) *
                                  _bivalentGraph.HeatLossCoefficient;
                    break;
                case Mode.PartiallyBivalent:
                    double midPointTemp = (_bivalentGraph.AdditionalSourceOnlyTemp +
                                           (_bivalentGraph.BivalentTemp - _bivalentGraph.AdditionalSourceOnlyTemp) / 2);
                    sugCapacity = (_bivalentGraph.ZeroLoadTemp - midPointTemp) *
                                  _bivalentGraph.HeatLossCoefficient;
                    break;
                default:
                    throw new NotImplementedException("Simulation mode not implemented");
            }

            textBlock.Text = "Suggested capacity " + sugCapacity;

            HeatPumpMaxCapacityTextBox.ToolTip = "Suggested capacity " + sugCapacity;
            SuggestedHeatPumpCapacityPopUp.Show();
        }

        private void SetSliderMaximum()
        {
            if (!GUIChecks.TryGetValue(AmbientMaxTEmpTxtBox, out double maxTemp) ||
                !GUIChecks.TryGetValue(AmbientMinTempTxtBox, out double minTemp)) return;

            CurrentTempSlider.Maximum = maxTemp;
            CurrentTempSlider.Minimum = minTemp;
        }

        private bool _isChanging;

        private void CurrentTempTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO check when the value is below minimal ambient temperature
            if (_isChanging)
                return;
            _isChanging = true;
            GUIChecks.ReplaceDot(sender, e);
            
            if (!GUIChecks.TryGetValue(CurrentTempTextBox, out double txtBoxValue, false))
            {
                _isChanging = false;
                return;
            }

            CurrentTempSlider.Value = txtBoxValue;
            _bivalentGraph.DrawCurrentHeatLoss(BivalentPointChart, txtBoxValue);
            CalculateUtilization();
            _isChanging = false;
        }

        private void CalculateUtilization()
        {
            if (_isInitializing)
                return;
            if (!GUIChecks.TryGetValue(CurrentHeatLossTextBox, out _bivalentUtilization.CurrentHeatLoss) ||
                !GUIChecks.TryGetValue(HeatPumpMaxCapacityTextBox, out _bivalentUtilization.HeatPumpMaxCapacity) ||
                !GUIChecks.TryGetValue(AdditionalHeatSourceMaxCapacityTextBox,
                    out _bivalentUtilization.AdditionalSourceMaxCapacity)) return;

            if (_mode != Mode.Monovalent)
            {
                _bivalentUtilization.CapacityAtBivalentPoint = _bivalentGraph.CapacityAtBivalentPoint;
                _bivalentUtilization.AdditionalHeatSourceOnlyCapacity = _bivalentGraph.AdditionalHeatSourceOnlyCapacity;
            }
            else
            {
                _bivalentUtilization.CapacityAtBivalentPoint = 0;
                _bivalentUtilization.AdditionalHeatSourceOnlyCapacity = 0;
            }

            //TODO check if the max capacity is not too low
            var utilization = _bivalentUtilization.Calculate(_mode);

            // if the capacity of heat pump is not enough
            HeatPumpUtilizationTextBox.ToolTip = "Actual utilization is " + utilization.Item1;
            if (utilization.Item1 > 100 && !_bivalentUtilization.HeatPumpOff)
            {
                HeatPumpUtilizationTextBox.Background = Brushes.Red;
                HeatPumpUtilizationTextBox.Text = "100";
            }
            else if (_bivalentUtilization.HeatPumpOff)
            {
                HeatPumpUtilizationTextBox.Background = Brushes.Gray;
                HeatPumpUtilizationTextBox.ToolTip += "\n OFF";
                HeatPumpUtilizationTextBox.Text = "0";
            }
            else
            {
                HeatPumpUtilizationTextBox.Background = Brushes.White;
                HeatPumpUtilizationTextBox.Text = utilization.Item1.ToString("F1");
                HeatPumpUtilizationTextBox.ToolTip = null;
            }

            // additional heat source
            AdditionalHeatSourceUtilizationTextBox.Background = utilization.Item2 > 100 ? Brushes.Red : Brushes.White;

            AdditionalHeatSourceUtilizationTextBox.Text = utilization.Item2.ToString("F1");
        }

        private void CurrentTempSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isChanging)
                return;
            _isChanging = true;
            CurrentTempTextBox.Text = CurrentTempSlider.Value.ToString("F1");
            _bivalentGraph.DrawCurrentHeatLoss(BivalentPointChart, CurrentTempSlider.Value);
            CalculateUtilization();
            _isChanging = false;
        }

        private void OnHeatLossChanged(double currentHeatLoss)
        {
            CurrentHeatLossTextBox.Text = currentHeatLoss.ToString("F1");
        }
    }
}