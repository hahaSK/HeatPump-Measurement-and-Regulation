using System;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace MainGUI
{
    public class BivalentGraph
    {
        public delegate void HeatLossChanged(double currentHeatLoss);

        public event HeatLossChanged heatLossChanged;

        public double AmbientMaxTemp,
            AmbientMinTemp,
            HeatLossCoefficient,
            ZeroLoadTemp,
            BivalentTemp,
            AdditionalSourceOnlyTemp;

        public double CapacityAtBivalentPoint { get; private set; }

        public double AdditionalHeatSourceOnlyCapacity { get; private set; }

        private readonly ScatterSeries _currentHeatLoss = new ScatterSeries
            {PointGeometry = DefaultGeometries.Cross, StrokeThickness = 2};

        private readonly SeriesCollection _heatLossCollection = new SeriesCollection
        {
            new LineSeries
            {
                Title = "Heat Loss",
                StrokeThickness = 0.8,
                PointGeometry = DefaultGeometries.Cross
            }
        };

        private readonly ColumnSeries _bivalentPointColumnSeries = new ColumnSeries
        {
            Title = "Bivalent point",
            StrokeThickness = 1,
            Fill = Brushes.Green,
            MaxColumnWidth = 3.5,
            Stroke = Brushes.Green,
            PointGeometry = DefaultGeometries.Cross
        };

        private readonly ColumnSeries _additionalHeatSourceOnlyPoint = new ColumnSeries
        {
            Title = "Additional heat source only",
            StrokeThickness = 1,
            Fill = Brushes.DarkOrange,
            MaxColumnWidth = 3.5,
            Stroke = Brushes.DarkOrange,
            PointGeometry = DefaultGeometries.Cross
        };

        public void RedrawGraph(CartesianChart graph, BivalentSimulation.Mode simulationMode)
        {
            //TODO check if the input makes sense like MAX > MIN etc...

            // setting minimal values of x Axis
            if (graph.AxisX.Count > 0)
            {
                graph.AxisX[0].MinValue = AmbientMinTemp;
                graph.AxisX[0].MaxValue = AmbientMaxTemp;
            }

            // line of object heat losses
            //TODO check if Min > Zero load temp
            double topPoint = HeatLossCoefficient * (ZeroLoadTemp - AmbientMinTemp);
            CapacityAtBivalentPoint = HeatLossCoefficient * (ZeroLoadTemp - BivalentTemp);
            _heatLossCollection[0].Values = new ChartValues<ObservablePoint>
            {
                new ObservablePoint(AmbientMinTemp, topPoint),
                new ObservablePoint(ZeroLoadTemp, 0)
            };

            if (simulationMode != BivalentSimulation.Mode.Monovalent)
            {
                _bivalentPointColumnSeries.Values = new ChartValues<ObservablePoint>
                {
                    new ObservablePoint(BivalentTemp, CapacityAtBivalentPoint)
                };
                if (graph.Series != null && graph.Series.Contains(_bivalentPointColumnSeries))
                {
                    _heatLossCollection.Remove(_bivalentPointColumnSeries);
                    _heatLossCollection.Add(_bivalentPointColumnSeries);
                }
                else
                    _heatLossCollection.Add(_bivalentPointColumnSeries);

                if (simulationMode == BivalentSimulation.Mode.PartiallyBivalent)
                {
                    AdditionalHeatSourceOnlyCapacity = HeatLossCoefficient * (ZeroLoadTemp - AdditionalSourceOnlyTemp);
                    _additionalHeatSourceOnlyPoint.Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(AdditionalSourceOnlyTemp, AdditionalHeatSourceOnlyCapacity)
                    };
                    if (graph.Series != null && graph.Series.Contains(_additionalHeatSourceOnlyPoint))
                    {
                        _heatLossCollection.Remove(_additionalHeatSourceOnlyPoint);
                        _heatLossCollection.Add(_additionalHeatSourceOnlyPoint);
                    }
                    else
                        _heatLossCollection.Add(_additionalHeatSourceOnlyPoint);
                }
                else // if the mode is not partially bivalent delete the line
                    _heatLossCollection.Remove(_additionalHeatSourceOnlyPoint);
            }
            else // if the mode is not partially bivalent delete the line
                _heatLossCollection.Remove(_additionalHeatSourceOnlyPoint);

            graph.Series = _heatLossCollection;
        }

        public void DrawCurrentHeatLoss(CartesianChart graph, double currentAmbientTemp)
        {
            if (graph.Series == null)
                return;

            double yValue = HeatLossCoefficient * (ZeroLoadTemp - currentAmbientTemp);
            if (yValue < 0)
                yValue = 0;
            
            _currentHeatLoss.Title = "Current heat loss";
            _currentHeatLoss.Values = new ChartValues<ObservablePoint>
            {
                new ObservablePoint(currentAmbientTemp, yValue)
            };

            if (graph.Series.Contains(_currentHeatLoss))
            {
                graph.Series.Remove(_currentHeatLoss);
                graph.Series.Add(_currentHeatLoss);
            }
            else
                graph.Series.Add(_currentHeatLoss);

            // draw the current heat loss point on-top of everything
            System.Windows.Controls.Panel.SetZIndex(_currentHeatLoss, 10);
            heatLossChanged?.Invoke(yValue);
        }
    }
}