using System;

namespace MainGUI
{
    public class BivalentUtilization
    {
        public double CurrentHeatLoss, HeatPumpMaxCapacity, AdditionalSourceMaxCapacity, CapacityAtBivalentPoint, AdditionalHeatSourceOnlyCapacity;

        public Tuple<double, double> Calculate(BivalentSimulation.Mode simulationMode)
        {
            double additionalSource = 0;
            double heatPumpUtilization = (CurrentHeatLoss / HeatPumpMaxCapacity) * 100;
            switch (simulationMode)
            {
                case BivalentSimulation.Mode.AlternativeBivalent when heatPumpUtilization > 100:
                    additionalSource = (CurrentHeatLoss / AdditionalSourceMaxCapacity) * 100;
                    break;
                case BivalentSimulation.Mode.AlternativeBivalent:
                    additionalSource = 0;
                    break;
                case BivalentSimulation.Mode.ParallelBivalent:
                    additionalSource = ((CurrentHeatLoss - HeatPumpMaxCapacity) / AdditionalSourceMaxCapacity) * 100;
                    if (additionalSource < 0)
                        additionalSource = 0;
                    break;
                case BivalentSimulation.Mode.PartiallyBivalent:
                    if (CurrentHeatLoss >= CapacityAtBivalentPoint && CurrentHeatLoss <= AdditionalHeatSourceOnlyCapacity)
                    {
                        double capacityDifference = CurrentHeatLoss - CapacityAtBivalentPoint;
                        // not % cause we use it in calculation below
                        heatPumpUtilization = (CurrentHeatLoss - capacityDifference / 2) / HeatPumpMaxCapacity;
                        additionalSource = (((CurrentHeatLoss - heatPumpUtilization * HeatPumpMaxCapacity)) / AdditionalSourceMaxCapacity) * 100;
                        // to %
                        heatPumpUtilization *= 100;
                    }
                    else if (CurrentHeatLoss > AdditionalHeatSourceOnlyCapacity)
                    {
                        additionalSource = (CurrentHeatLoss / AdditionalSourceMaxCapacity) * 100;
                        // signalizing that the heat pump is OFF
                        heatPumpUtilization = -1;
                    }
                    else
                        additionalSource = 0;
                    break;
                 
            }

            heatPumpUtilization = Math.Round(heatPumpUtilization, 3);
            additionalSource = Math.Round(additionalSource, 3);
            return new Tuple<double, double>(heatPumpUtilization, additionalSource);
        }
    }
}