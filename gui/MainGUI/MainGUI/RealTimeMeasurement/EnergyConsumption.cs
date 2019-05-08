using System;

namespace MainGUI.RealTimeMeasurement
{
    public class EnergyConsumption
    {
        private double _energySum;
        private uint _callCount = 0;
        
        public double AddValue(double timeElapsed, MeasurementData measurementData)
        {
            _callCount += 1;

            double capacity = measurementData.U * measurementData.I;
            _energySum += (capacity / 1000);
            double average = _energySum / _callCount;
            // to kWh
            return Math.Round(average * (timeElapsed / 3600), 6);
        }

        public void RestCount()
        {
            _callCount = 0;
        }
    }
}
