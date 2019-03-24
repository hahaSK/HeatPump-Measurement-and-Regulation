using System;
using System.Linq;

namespace MainGUI
{
    public class SystemCalculation
    {
        internal struct SystemData
        {
            internal double COP;
            internal double Phe;
            internal double Pe;
        }

        public CoilData CondenserData;
        public ElectricalApplianceData CompressorData;

        private readonly Coil _condenser = new Coil();
        private readonly ElectricalAppliance _compressor = new ElectricalAppliance();
        private readonly CalculateCOPandP _calculateCoPandP = new CalculateCOPandP();

        private static SystemData _systemData;

        public static double GetSysCOP => _systemData.COP;
        public static double GetSysPhe => _systemData.Phe;
        public static double GetSysPe => _systemData.Pe;

        /// <summary>
        /// Initiate calculation of the whole system
        /// </summary>
        internal SystemData? CalculateSystem()
        {
            // Condenser
            double CondP = CondenserData.P ?? _condenser.CalcCoil(CondenserData);

            // Electrical appliances
            // TODO resize when Electrical appliance is destroyed
            double[] electricalAppliancesP = new double[ElectricalAppliance.numberOfInstances];
            int i = 0;
            electricalAppliancesP[i++] = CompressorData.P ?? _compressor.CalcAppliance(CompressorData.U.Value, CompressorData.I.Value);

            double electricalConsumption = electricalAppliancesP.Sum();

            double? COPResult = _calculateCoPandP.CalculateCOP(CondP, electricalConsumption);

            // TODO too much ?!
            if (COPResult != null)
                COPResult = Math.Round(COPResult.Value, 3);
            else
                return null;
            
            _systemData.COP = COPResult.Value;
            _systemData.Pe = electricalConsumption;
            // Heat capacity is equal to condenser capacity
            _systemData.Phe = CondP;

            return _systemData;
        }
    }
}