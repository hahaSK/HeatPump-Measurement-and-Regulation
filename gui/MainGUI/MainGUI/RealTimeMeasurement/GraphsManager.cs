using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace MainGUI.RealTimeMeasurement
{
    public class GraphsManager
    {
        private readonly CartesianChart _energyConsumptionGraph;

        public GraphsManager(CartesianChart energyConsumptionGraph)
        {
            _energyConsumptionGraph = energyConsumptionGraph;
        }

        public void EnergyConsumptionNewPoint(double timeElapsed, double energyConsumption)
        {
            AddNewPoint(_energyConsumptionGraph, timeElapsed, energyConsumption);
        }

        private void AddNewPoint(CartesianChart graph, double x, double y)
        {
            if (graph.Series?[0].Values != null)
            {
                ObservablePoint observablePoint = new ObservablePoint(x, y);
                graph.Series[0].Values.Add(observablePoint);
            }
            else if (graph.Series != null)
            {
                graph.Dispatcher.Invoke(() =>
                    graph.Series[0].Values = new ChartValues<ObservablePoint> {new ObservablePoint(x, y)});
            }
        }

        public void ClearGraphs()
        {
            _energyConsumptionGraph.Series[0]?.Values?.Clear();
        }

        public void ResetView()
        {
            //Use the axis MinValue/MaxValue properties to specify the values to display.
            //use double.Nan to clear it.

            _energyConsumptionGraph.AxisX[0].MinValue = double.NaN;
            _energyConsumptionGraph.AxisX[0].MaxValue = double.NaN;
            _energyConsumptionGraph.AxisY[0].MinValue = double.NaN;
            _energyConsumptionGraph.AxisY[0].MaxValue = double.NaN;
        }
    }
}