using System;
using System.Windows.Navigation;

namespace MainGUI
{
    public struct CoilData
    {
        public double? Qm;
        public double? DeltaT;
        public double? T1;
        public double? T2;
        public double? P;

        public CoilData(double p)
        {
            Qm = null;
            DeltaT = null;
            T1 = null;
            T2 = null;
            P = p;
        }

        public CoilData(double qm, double? deltaT, double? t1, double? t2)
        {
            Qm = qm;
            DeltaT = deltaT;
            T1 = t1;
            T2 = t2;
            P = null;
        }
    }

    public class Coil
    {
        private CalculateCOPandP calculateCOPandP = new CalculateCOPandP();

        public double CalcCoil(CoilData coilData)
        {
            var P = coilData.DeltaT != null ? CalcCoil((double)coilData.Qm, (double) coilData.DeltaT) : CalcCoil((double) coilData.Qm, (double) coilData.T1, (double) coilData.T2);

            return P ?? 0;
        }

        /// <summary>
        /// Calculate coil heat capacity
        /// </summary>
        /// <param name="Qm">mass flow rate [m3/s]</param>
        /// <param name="deltaT">temperature difference [K]</param>
        /// <returns>Heat Capacity [W]. <see cref="T:null" /> if DLL is not found.</returns>
        private double? CalcCoil(double Qm, double deltaT)
        {
            // recalculate to [kg/s]
            Qm *= Constants.WaterDensity;
            return calculateCOPandP.CalculatePhe(Qm, Constants.CpH2O, deltaT);
        }

        /// <summary>
        /// Calculate coil heat capacity 
        /// </summary>
        /// <param name="Qm">mass flow rate [m3/s]</param>
        /// <param name="t1">temperature [T] or [°C]</param>
        /// <param name="t2">temperature [T] or [°C]</param>
        /// <returns>Heat Capacity [W]. <see cref="T:null" /> if DLL is not found.</returns>
        private double? CalcCoil(double Qm, double t1, double t2)
        {
            // recalculate to [kg/s]
            Qm *= Constants.WaterDensity;
            return calculateCOPandP.CalculatePhe(Qm, Constants.CpH2O, t1, t2);
        }
    }
}