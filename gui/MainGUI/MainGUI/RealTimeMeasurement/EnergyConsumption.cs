using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return Math.Round(average * (timeElapsed / 3600), 1);
        }
        
    }
}
