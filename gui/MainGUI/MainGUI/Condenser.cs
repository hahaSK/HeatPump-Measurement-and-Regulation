using System;
using System.Windows.Navigation;

namespace MainGUI
{
    public struct CondenserData
    {
        public double? Qm;
        public double? DeltaT;
        public double? T1;
        public double? T2;
        public double? P;

        public CondenserData(double p)
        {
            Qm = null;
            DeltaT = null;
            T1 = null;
            T2 = null;
            P = p;
        }

        public CondenserData(double qm, double? deltaT, double? t1, double? t2)
        {
            Qm = qm;
            DeltaT = deltaT;
            T1 = t1;
            T2 = t2;
            P = null;
        }
    }

    public class Condenser
    {
        // Water density [kg/m3]
        public const int WaterDensity = 997;

        // Specific heat capacity of water [J/kg°C]
        public const double CpH2O = 4186;

        private CalculateCOPandP calculateCOPandP = new CalculateCOPandP();

        public double CalcCondenser(CondenserData condenserData)
        {
            var P = condenserData.DeltaT != null ? CalcCondenser((double)condenserData.Qm, (double) condenserData.DeltaT) : CalcCondenser((double) condenserData.Qm, (double) condenserData.T1, (double) condenserData.T2);

            return P ?? 0;
        }

        /// <summary>
        /// Calculate Condenser heat capacity
        /// </summary>
        /// <param name="Qm">mass flow rate [m3/s]</param>
        /// <param name="deltaT">temperature difference [K]</param>
        /// <returns>Heat Capacity [W]. <see cref="T:null" /> if DLL is not found.</returns>
        private double? CalcCondenser(double Qm, double deltaT)
        {
            // recalculate to [kg/s]
            Qm *= WaterDensity;
            return calculateCOPandP.CalculatePhe(Qm, CpH2O, deltaT);
        }

        /// <summary>
        /// Calculate Condenser heat capacity 
        /// </summary>
        /// <param name="Qm">mass flow rate [m3/s]</param>
        /// <param name="t1">temperature [T] or [°C]</param>
        /// <param name="t2">temperature [T] or [°C]</param>
        /// <returns>Heat Capacity [W]. <see cref="T:null" /> if DLL is not found.</returns>
        private double? CalcCondenser(double Qm, double t1, double t2)
        {
            // recalculate to [kg/s]
            Qm *= WaterDensity;
            return calculateCOPandP.CalculatePhe(Qm, CpH2O, t1, t2);
        }
    }
}