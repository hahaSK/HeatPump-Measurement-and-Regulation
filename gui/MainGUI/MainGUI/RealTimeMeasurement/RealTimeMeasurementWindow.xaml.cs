using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace MainGUI.RealTimeMeasurement
{
    /// <summary>
    /// Interaction logic for RealTimeMeasurementWindow.xaml
    /// </summary>
    public partial class RealTimeMeasurementWindow : Window
    {
        public ConsoleManager ConsoleManager => _consoleManager;

        private readonly OpenFileDialog _openFileDialog = new OpenFileDialog();
        private readonly RealTimeMeasurementData _realTimeMeasurementData;
        private readonly ConsoleManager _consoleManager;
        private readonly SystemCalculation _systemCalculation = new SystemCalculation();
        private readonly GraphsManager _graphsManager;
        private readonly EnergyConsumption _energyConsumption = new EnergyConsumption();

        private readonly Timer _loadFileTimer = new Timer();

        private bool _simulating = false;
        private string _fileName;
        private bool _evaporatorIsChecked;
        private uint _timeElapsed, _interval;

        public RealTimeMeasurementWindow()
        {
            InitializeComponent();
            _consoleManager = new ConsoleManager(ConsoleTextBox);

            _realTimeMeasurementData = new RealTimeMeasurementData(this);
            _graphsManager = new GraphsManager(EnergyConsumpsionGraph);

            CalculateEvaporatorChkbox_Clicked(this, new RoutedEventArgs());
        }

        ~RealTimeMeasurementWindow()
        {
            _loadFileTimer.Stop();
            _loadFileTimer.Close();
        }

        private void ChooseFileBtn_Click(object sender, RoutedEventArgs e)
        {
            _openFileDialog.Filter = "Text files (*.txt)|*.txt";
            bool? result = _openFileDialog.ShowDialog();
            if (result == true)
                PathLabel.Content = _fileName = _openFileDialog.FileName;
        }

        private void StartStopSimulationBtn_Click(object sender, RoutedEventArgs e)
        {
            // checking the input parameters
            if (!GUIChecks.TryGetValue(LoadIntervalTextBox, out _interval))
                return;

            if (_fileName == string.Empty || !File.Exists(_fileName))
            {
                ErrorPrinting.PrintError("File" + _fileName + " doesn't exist. Please choose different file.");
                return;
            }

            if (_interval == 0)
            {
                ConsoleManager.ConsoleWriteMessage("Interval cannot be 0. Setting it to 1!");
                _interval = 1;
            }

            if (!_simulating)
                StartSimulation(_interval);
            else if (_simulating)
                StopSimulation();

            _simulating = !_simulating;
        }

        private void StartSimulation(uint interval)
        {
            StartStopSimulationBtn.Content = "Stop";
            ChooseFileBtn.IsEnabled = false;
            LoadIntervalTextBox.IsReadOnly = true;

            _consoleManager.ClearConsole();
            _timeElapsed = 0;
            _graphsManager.ClearGraphs();
            _energyConsumption.RestCount();

            // milliseconds to seconds
            _loadFileTimer.Interval = interval * 1000;
            _loadFileTimer.Elapsed += TimerElapsed;
            _loadFileTimer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            var result = _realTimeMeasurementData.ReadData(_fileName);
            if (!result.Item1)
                return;

            _timeElapsed += _interval;

            DataToUi(result.Item2);
            CalculateCOP(result.Item2);

            // calculate energy consumption and add point to the graph
            double consumption = _energyConsumption.AddValue(_timeElapsed, result.Item2);
            EnergyConsumptionTextBox.Dispatcher.Invoke(() =>
                EnergyConsumptionTextBox.Text = consumption.ToString("F"));
            _graphsManager.EnergyConsumptionNewPoint(_timeElapsed, consumption);

            if (_evaporatorIsChecked)
                CalculateEvaporator(result.Item2);
        }

        private void CalculateCOP(MeasurementData measurementData)
        {
            //Condenser
            // loaded volume flow is in cm3 so need to convert it to m3
            double condenserV = measurementData.v * Math.Pow(10, -6);
            _systemCalculation.CondenserData =
                new CoilData(condenserV, null, measurementData.TinW, measurementData.ToutW);

            // Electrical appliances
            _systemCalculation.CompressorData = new ElectricalApplianceData(measurementData.U, measurementData.I);

            if (!_systemCalculation.CalculateSystem().HasValue)
            {
                _consoleManager.ConsoleWriteMessage("COP not calculated.");
                COPCalculatedTextBox.Dispatcher.Invoke(() => COPCalculatedTextBox.Text = "0");
                return;
            }

            COPCalculatedTextBox.Dispatcher.Invoke(() =>
                COPCalculatedTextBox.Text = SystemCalculation.GetSysCOP.ToString("F"));
        }

        private void CalculateEvaporator(MeasurementData measurementData)
        {
            var evaporatorVTextBox = EvaporatorVTextBox.Dispatcher.Invoke(() => EvaporatorVTextBox.Text);
            if (!double.TryParse(evaporatorVTextBox, out double evaporatorV))
            {
                _consoleManager.ConsoleWriteMessage("Cannot parse Evaporator volume flow.");
                return;
            }

            var evaporatorHumidity = EvaporatorHumidity.Dispatcher.Invoke(() => EvaporatorHumidity.Text);
            if (!double.TryParse(evaporatorHumidity, out double humidity))
            {
                _consoleManager.ConsoleWriteMessage("Cannot parse humidity.");
                return;
            }

            var heightOverSea = HeightOverSea.Dispatcher.Invoke(() => HeightOverSea.Text);
            if (!double.TryParse(heightOverSea, out double height))
            {
                _consoleManager.ConsoleWriteMessage("Cannot parse height over sea.");
                return;
            }

            _systemCalculation.EvaporatorData =
                new CoilData(null, measurementData.TinA - measurementData.ToutA, measurementData.TinA);
            double? result = _systemCalculation.CalculateEvaporator(humidity, height, evaporatorV);
            EvaporatorPTextBox.Dispatcher.Invoke(() => EvaporatorPTextBox.Text = result.ToString());
        }

        private void StopSimulation()
        {
            _loadFileTimer.Stop();
            _loadFileTimer.Close();
            _loadFileTimer.Elapsed -= TimerElapsed;
            LoadIntervalTextBox.IsReadOnly = false;

            StartStopSimulationBtn.Content = "Start";
            ChooseFileBtn.IsEnabled = true;
        }

        private void LoadIntervalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !GUIChecks.IsTextAllowedForUint(e.Text);
        }

        private void LoadIntervalTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !GUIChecks.IsSpaceAllowed(e);
        }

        private void DataToUi(MeasurementData measurementData)
        {
            Tc1TextBox.Dispatcher.Invoke(() => Tc1TextBox.Text = measurementData.Tc1.ToString("F"));
            Tc2TextBox.Dispatcher.Invoke(() => Tc2TextBox.Text = measurementData.Tc2.ToString("F"));
            Tc3TextBox.Dispatcher.Invoke(() => Tc3TextBox.Text = measurementData.Tc3.ToString("F"));
            Tc4TextBox.Dispatcher.Invoke(() => Tc4TextBox.Text = measurementData.Tc4.ToString("F"));

            TempAirInTextBox.Dispatcher.Invoke(() => TempAirInTextBox.Text = measurementData.TinA.ToString("F"));
            TempAirOutTextBox.Dispatcher.Invoke(() => TempAirOutTextBox.Text = measurementData.ToutA.ToString("F"));

            TempWaterInTextBox.Dispatcher.Invoke(() => TempWaterInTextBox.Text = measurementData.TinW.ToString("F"));
            TempWaterOutTextBox.Dispatcher.Invoke(() =>
                TempWaterOutTextBox.Text = measurementData.ToutW.ToString("F"));

            WaterTankTemperatureTextBox.Dispatcher.Invoke(() =>
                WaterTankTemperatureTextBox.Text = measurementData.Tvn.ToString("F"));

            WaterVolumeFlowTextBox.Dispatcher.Invoke(() =>
                WaterVolumeFlowTextBox.Text = measurementData.v.ToString("F"));

            UTextBox.Dispatcher.Invoke(() => UTextBox.Text = measurementData.U.ToString("F"));
            ITextBox.Dispatcher.Invoke(() => ITextBox.Text = measurementData.I.ToString("F"));

            COPLoadedTextBox.Dispatcher.Invoke(() => COPLoadedTextBox.Text = measurementData.COP.ToString("F"));
        }

        private void TextBoxOnChange(object sender, TextChangedEventArgs e)
        {
            GUIChecks.ReplaceDot(sender, e);
        }

        private void TextBoxTextPreview(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !GUIChecks.IsTextAllowedForDoubleNonNegative(e.Text);
        }

        private void TextBoxKeyDownPreview(object sender, KeyEventArgs e)
        {
            e.Handled = !GUIChecks.IsSpaceAllowed(e);
        }

        private void CalculateEvaporatorChkbox_Clicked(object sender, RoutedEventArgs e)
        {
            if (!CalculateEvaporatorChkbox.IsChecked.HasValue)
                return;

            if (CalculateEvaporatorChkbox.IsChecked.Value)
                EvaporatorHumidityCanvas.Visibility = EvaporatorVCanvas.Visibility =
                    EvaporatorPCanvas.Visibility = HeightOverSeaCanvas.Visibility = Visibility.Visible;
            else
                EvaporatorHumidityCanvas.Visibility = EvaporatorVCanvas.Visibility =
                    EvaporatorPCanvas.Visibility = HeightOverSeaCanvas.Visibility = Visibility.Hidden;

            _evaporatorIsChecked = CalculateEvaporatorChkbox.IsChecked.Value;
        }

        private void ResetViewButton_Click(object sender, RoutedEventArgs e)
        {
            _graphsManager.ResetView();
        }

        private void Tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Tab.SelectedIndex)
            {
                // Data window
                case 0:
                    DataCanvas.Dispatcher.Invoke(() => DataCanvas.Visibility = Visibility.Visible);
                    GraphsCanvas.Dispatcher.Invoke(() => GraphsCanvas.Visibility = Visibility.Hidden);
                    break;
                // Graphs window
                case 1:
                    DataCanvas.Dispatcher.Invoke(() => DataCanvas.Visibility = Visibility.Hidden);
                    GraphsCanvas.Dispatcher.Invoke(() => GraphsCanvas.Visibility = Visibility.Visible);
                    break;

                default:
                    throw new NotImplementedException("Tab selection not implemented.");
            }
        }
    }
}