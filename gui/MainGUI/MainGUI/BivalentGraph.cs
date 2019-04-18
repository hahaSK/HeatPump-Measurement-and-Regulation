using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace MainGUI
{
    public class BivalentGraph
    {
        private readonly Axis _myXAxis = new Axis();
        public double AmbientMaxTemp, AmbientMinTemp, HeatLossCoefficient, ZeroLoadTemp, BivalentTemp;

        public void RedrawGraph(CartesianChart graph, BivalentSimulation.Mode simulationMode)
        {
            //TODO check if the input makes sense like MAX > MIN etc...

            // setting minimal values of x Axis
            _myXAxis.MinValue = AmbientMinTemp;
            _myXAxis.MaxValue = AmbientMaxTemp;
            graph.AxisX.Clear();
            graph.AxisX.Add(_myXAxis);

            // line of object heat losses
            //TODO check if Min > Zero load temp
            double topPoint = HeatLossCoefficient * (ZeroLoadTemp - AmbientMinTemp);
            // where does the bivalent point crosses the line
            double crossCurvePoint = HeatLossCoefficient * (ZeroLoadTemp - BivalentTemp);
            SeriesCollection heatLossCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Heat Loss",
                    StrokeThickness = 0.8,
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(AmbientMinTemp, topPoint),
                        new ObservablePoint(ZeroLoadTemp, 0)
                    }
                }
            };

            if (simulationMode != BivalentSimulation.Mode.Monovalent)
            {
                heatLossCollection.Add(
                    new ColumnSeries
                    {
                        Title = "Bivalent point",
                        StrokeThickness = 0.8,
                        Values = new ChartValues<ObservablePoint>
                        {
                            new ObservablePoint(BivalentTemp, crossCurvePoint)
                        }
                    }
                );
            }

            graph.Series = heatLossCollection;
        }
    }
}