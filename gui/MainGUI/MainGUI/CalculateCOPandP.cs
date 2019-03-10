using System;

namespace MainGUI
{
    public class CalculateCOPandP
    {
        /// <summary>
        /// Calculates the COP (Coefficient of Performance)
        /// </summary>
        /// <param name="Phe">Phe Heat Capacity [W]</param>
        /// <param name="Pe">Pe Electrical Capacity [W]</param>
        /// <returns>COP. <see cref="T:null"/> if DLL is not found</returns>
        public double? CalculateCOP(double Phe, double Pe)
        {
            return COPandPCalcLib.CalcCOP(Phe, Pe);
        }

        /// <summary>
        /// Calculates the Heat Capacity of given substance
        /// </summary>
        /// <param name="Qm">mass flow rate [kg/s]</param>
        /// <param name="Cp">specific thermal capacity of given substance [J/KgK]</param>
        /// <param name="DeltaT">temperature difference [K]</param>
        /// <returns>Heat Capacity [W]. <see cref="T:null" /> if DLL is not found.</returns>
        public double? CalculatePhe(double Qm, double Cp, double DeltaT)
        {
            return COPandPCalcLib.CalcPhe(Qm, Cp, DeltaT);
        }

        /// <summary>
        /// Calculates the Heat Capacity of given substance
        /// </summary>
        /// <param name="Qm">mass flow rate [kg/s]</param>
        /// <param name="Cp">specific thermal capacity of given substance [J/KgK]</param>
        /// <param name="T1">temperature [T] or [°C]</param>
        /// <param name="T2">temperature [T] or [°C]</param>
        /// <returns>- Heat Capacity [W]. <see cref="T:null" /> if DLL is not found.</returns>
        public double? CalculatePhe(double Qm, double Cp, double T1, double T2)
        {
            return COPandPCalcLib.CalcPhe(Qm, Cp, T1, T2);
        }

        /// <summary>
        /// Calculates the (consumed) Electrical capacity
        /// </summary>
        /// <param name="U">Voltage</param>
        /// <param name="I">Current</param>
        /// <returns>Electrical capacity. <see cref="T:null" /> if DLL is not found.</returns>
        public double? CalculatePe(double U, double I)
        {
            return COPandPCalcLib.CalcPe(U, I);
        }
    }
}