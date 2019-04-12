using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace MainGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GUIChecks guiChecks = new GUIChecks();
        private SystemCalculation _systemCalculation = new SystemCalculation();
        private CompareWindow _compareWindow;

        private readonly string[] _compressorOptions = {"Capacity", "U, I"};
        private readonly string[] _condenseDataOptions = {"Delta T", "Tin,W ; Tout,W", "P"};
        private readonly string[] _flowUnitsOptions = {"m3/h", "m3/s"};

        private bool _calculationDone;

        public MainWindow()
        {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            // Set globalization of current thread
            CultureInfo ci = new CultureInfo("sk-SK");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            InitializeComponent();

            CompressorInputOptions.ItemsSource = _compressorOptions;
            CondDataOptions.ItemsSource = _condenseDataOptions;
            CondFlowUnitOptions.ItemsSource = _flowUnitsOptions;
            EvapFlowUnitOptions.ItemsSource = _flowUnitsOptions;

            CalculateEvaporatorChkbox_Clicked(this, new RoutedEventArgs());
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            bool condDataSet, electricAppDataSet;
            //Condenser 
            condDataSet = SetCondenserData();

            // Electrical appliances
            electricAppDataSet = SetElectricalApplianceData();

            if (!condDataSet || !electricAppDataSet)
            {
                ErrorPrinting.PrintError("Couldn't set data");
                return;
            }

            if (!_systemCalculation.CalculateSystem().HasValue)
                return;

            SystemCOPResultTextBox.Text = SystemCalculation.GetSysCOP.ToString("F1");

            SetEvaporator();

            _calculationDone = true;
        }

        /// <summary>
        /// Set electrical applianceData
        /// </summary>
        /// <returns><see cref="T:false"/> if encounter problem</returns>
        private bool SetElectricalApplianceData()
        {
            if (CompressorCapacityCanvas.Visibility == Visibility.Visible)
            {
                if (!guiChecks.TryGetValue(CompressorCapacityTextBox, out double compressorP)) return false;
                _systemCalculation.CompressorData = new ElectricalApplianceData(compressorP);
                return true;
            }

            if (!guiChecks.TryGetValue(CompressorUTextBox, out double compressorU) ||
                !guiChecks.TryGetValue(CompressorITextBox, out double compressorI)) return false;
            _systemCalculation.CompressorData = new ElectricalApplianceData(compressorU, compressorI);
            return true;
        }

        /// <summary>
        /// Set Condenser data into struct
        /// </summary>
        /// <returns><see cref="T:false"/> if encounter problem</returns>
        private bool SetCondenserData()
        {
            // if user knows P of condenser
            if (CondenserPCanvas.Visibility == Visibility.Visible)
            {
                if (!guiChecks.TryGetValue(CondenserP, out double condenserP)) return false;
                _systemCalculation.CondenserData = new CoilData(condenserP);
                return true;
            }

            if (!guiChecks.TryGetValue(CondenserV, out double condenserV))
                return false;

            // m3/h
            if (CondFlowUnitOptions.SelectedIndex == 0)
                condenserV /= 3600;

            switch (DeltaTCanvas.Visibility)
            {
                case Visibility.Visible:
                    if (!guiChecks.TryGetValue(CondenserDeltaT, out double condenserDeltaT)) return false;
                    _systemCalculation.CondenserData = new CoilData(condenserV, condenserDeltaT, null, null);
                    break;
                case Visibility.Hidden:
                    if (!guiChecks.TryGetValue(CondenserT1, out double condenserT1) ||
                        !guiChecks.TryGetValue(CondenserT2, out double condenserT2)) return false;
                    _systemCalculation.CondenserData = new CoilData(condenserV, null, condenserT1, condenserT2);
                    break;
            }

            return true;
        }

        private void SetEvaporator()
        {
            if (!CalculateEvaporatorChkbox.IsChecked.HasValue || !CalculateEvaporatorChkbox.IsChecked.Value)
                return;

            if (SetEvaporatorData() && guiChecks.TryGetValue(EvaporatorHumidity, out double humidity) &&
                guiChecks.TryGetValue(HeightOverSea, out double height) &&
                guiChecks.TryGetValue(EvaporatorV, out double volumeFlow))
            {
                // m3/h
                if (EvapFlowUnitOptions.SelectedIndex == 0)
                    volumeFlow /= 3600;

                EvaporatorResultP.Text = _systemCalculation.CalculateEvaporator(humidity, height, volumeFlow).ToString();
            }
        }

        private bool SetEvaporatorData()
        {
            if (!guiChecks.TryGetValue(EvaporatorTin, out double evaporatorTin) ||
                !guiChecks.TryGetValue(EvaporatorTout, out double evaporatorTout)) return false;


            _systemCalculation.EvaporatorData = new CoilData(null, evaporatorTin - evaporatorTout, evaporatorTin);

            return true;
        }

        private void CompressorInputOptions_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (CompressorInputOptions.SelectedIndex)
            {
                case 0:
                    CompressorCapacityCanvas.Visibility = Visibility.Visible;
                    CompressorUICanvas.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    CompressorCapacityCanvas.Visibility = Visibility.Hidden;
                    CompressorUICanvas.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void CondDataOptions_SelectionChanged(object sender,
            System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (CondDataOptions.SelectedIndex)
            {
                case 0:
                    CondenserVCanvas.Visibility = Visibility.Visible;
                    DeltaTCanvas.Visibility = Visibility.Visible;
                    T1T2Canvas.Visibility = Visibility.Hidden;
                    CondenserPCanvas.Visibility = Visibility.Hidden;
                    break;
                case 1:
                    CondenserVCanvas.Visibility = Visibility.Visible;
                    DeltaTCanvas.Visibility = Visibility.Hidden;
                    T1T2Canvas.Visibility = Visibility.Visible;
                    CondenserPCanvas.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    CondenserVCanvas.Visibility = Visibility.Hidden;
                    DeltaTCanvas.Visibility = Visibility.Hidden;
                    T1T2Canvas.Visibility = Visibility.Hidden;
                    CondenserPCanvas.Visibility = Visibility.Visible;
                    break;
            }
        }

        #region TextBoxTextChanged

        private void CondenserV_TextChanged(object sender, TextChangedEventArgs e)
        {
            guiChecks.ReplaceDot(sender, e);
        }

        private void CondenserDeltaT_TextChanged(object sender, TextChangedEventArgs e)
        {
            guiChecks.ReplaceDot(sender, e);
        }

        private void CompressorCapacityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            guiChecks.ReplaceDot(sender, e);
        }

        private void CompressorUTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            guiChecks.ReplaceDot(sender, e);
        }

        private void CompressorITextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            guiChecks.ReplaceDot(sender, e);
        }

        private void CondenserT1_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            guiChecks.ReplaceDot(sender, e);
        }

        private void CondenserT2_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            guiChecks.ReplaceDot(sender, e);
        }

        private void CondenserP_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            guiChecks.ReplaceDot(sender, e);
        }

        #endregion

        private void ShowCompareWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (_compareWindow != null && _compareWindow.IsVisible)
            {
                _compareWindow.Activate();
                return;
            }

            if (_calculationDone)
            {
                _compareWindow = new CompareWindow(SystemCalculation.GetSysCOP, SystemCalculation.GetSysPhe,
                    SystemCalculation.GetSysPe);
                _compareWindow.Show();
            }
            else
                ErrorPrinting.PrintError("First Calculate system.");
        }

        private void CalculateEvaporatorChkbox_Clicked(object sender, RoutedEventArgs e)
        {
            if (!CalculateEvaporatorChkbox.IsChecked.HasValue)
                return;

            if (CalculateEvaporatorChkbox.IsChecked.Value)
                EvaporatorPCanvas.Visibility =
                    EvaporatorTCanvas.Visibility = EvaporatorVCanvas.Visibility = Visibility.Visible;
            else
                EvaporatorPCanvas.Visibility =
                    EvaporatorTCanvas.Visibility = EvaporatorVCanvas.Visibility = Visibility.Hidden;
        }
    }
}