using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            Bivalent
        }

        private readonly string[] _modeNames = {"Monovalent", "Alternative bivalent", "Parallel bivalent"};

        private readonly BivalentGraph _bivalentGraph = new BivalentGraph();
        private Mode _mode = Mode.Monovalent;

        public BivalentSimulation()
        {
            InitializeComponent();

            ModeCombBox.ItemsSource = _modeNames;

            SetModeSchema();

            BivalentPointChart.LegendLocation = LegendLocation.Left;

            // always will be 0
            Axis myYAxis = new Axis {MinValue = 0};
            BivalentPointChart.AxisY.Clear();
            BivalentPointChart.AxisY.Add(myYAxis);

            RedrawGraph();
        }

        private void RedrawGraph()
        {
            if (!GUIChecks.TryGetValue(AmbientMaxTEmpTxtBox, out _bivalentGraph.AmbientMaxTemp) ||
                !GUIChecks.TryGetValue(AmbientMinTempTxtBox, out _bivalentGraph.AmbientMinTemp) ||
                !GUIChecks.TryGetValue(ObjectHeatLossCoeffTxtBox, out _bivalentGraph.HeatLossCoefficient) ||
                !GUIChecks.TryGetValue(ZeroLoadTempTxtBox, out _bivalentGraph.ZeroLoadTemp)) return;
            if (_mode != Mode.Monovalent && !GUIChecks.TryGetValue(BivalentTempTxtBox, out _bivalentGraph.BivalentTemp)) return;

            _bivalentGraph.RedrawGraph(BivalentPointChart, _mode);
        }

        private void SetModeSchema()
        {
            switch (_mode)
            {
                case Mode.Monovalent:
                    MonovalentSchemaCanvas.Visibility = Visibility.Visible;
                    BivalentSchemaCanvas.Visibility = Visibility.Hidden;
                    BivalentTempCanvas.Visibility = Visibility.Hidden;
                    break;
                case Mode.Bivalent:
                    MonovalentSchemaCanvas.Visibility = Visibility.Hidden;
                    BivalentSchemaCanvas.Visibility = Visibility.Visible;
                    BivalentTempCanvas.Visibility = Visibility.Visible;
                    break;

                default:
                    throw new NotImplementedException("Mode not implemented");
            }
        }

        private void ModeCombBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _mode = ModeCombBox.SelectedIndex == 0 ? Mode.Monovalent : Mode.Bivalent;
            SetModeSchema();
        }

        private void TextBoxOnChange(object sender, TextChangedEventArgs e)
        {
            GUIChecks.ReplaceDot(sender, e);
        }

        private void TextBoxTextPreview(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !GUIChecks.IsTextAllowed(e.Text);
        }
    }
}