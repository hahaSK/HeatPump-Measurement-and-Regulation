using System;

namespace MainGUI
{
    /// <summary>
    /// Class calculates utilization of system.
    /// </summary>
    public class BivalentUtilization
    {
        public bool HeatPumpOff { get; private set; }

        public double CurrentHeatLoss,
            HeatPumpMaxCapacity,
            AdditionalSourceMaxCapacity,
            CapacityAtBivalentPoint,
            AdditionalHeatSourceOnlyCapacity;

        public Tuple<double, double> Calculate(BivalentSimulation.Mode simulationMode)
        {
            double additionalSource = 0;
            double heatPumpUtilization = (CurrentHeatLoss / HeatPumpMaxCapacity) * 100;
            HeatPumpOff = false;
            switch (simulationMode)
            {
                // calculate additional source only when we cross bivalent point
                case BivalentSimulation.Mode.AlternativeBivalent when CurrentHeatLoss > CapacityAtBivalentPoint:
                    additionalSource = (CurrentHeatLoss / AdditionalSourceMaxCapacity) * 100;
                    HeatPumpOff = true;
                    break;
                case BivalentSimulation.Mode.AlternativeBivalent:
                    additionalSource = 0;
                    break;
                // in parallel bivalent mode add additional source only when we cross bivalent point
                case BivalentSimulation.Mode.ParallelBivalent when CurrentHeatLoss > CapacityAtBivalentPoint:
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

                        // heat pump doesn't have spare capacity
                        // so the additional heat source is covering the whole heat loss BUT heat pump is still ON
                        if (heatPumpUtilization > 1)
                            additionalSource = ((CurrentHeatLoss - HeatPumpMaxCapacity) / AdditionalSourceMaxCapacity) * 100;
                        else
                        {
                            // heat pump has some spare capacity
                            additionalSource = (((CurrentHeatLoss - heatPumpUtilization * HeatPumpMaxCapacity)) /
                                                AdditionalSourceMaxCapacity) * 100;
                        }
                        
                        // to %
                        heatPumpUtilization *= 100;
                    }
                    else if (CurrentHeatLoss > AdditionalHeatSourceOnlyCapacity)
                    {
                        additionalSource = (CurrentHeatLoss / AdditionalSourceMaxCapacity) * 100;
                        // signalizing that the heat pump is OFF
                        HeatPumpOff = true;
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