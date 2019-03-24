using System;
using System.Runtime.InteropServices;

namespace MainGUI
{
    public static class COPandPCalcLib
    {       
        [DllImport("COPandPCalc", CallingConvention = CallingConvention.Cdecl)]
        private static extern double CalculateCOP(double Phe, double Pe);
        [DllImport("COPandPCalc", CallingConvention = CallingConvention.Cdecl)]
        private static extern double CalculatePhe(double Qm, double Cp, double DeltaT);
        [DllImport("COPandPCalc", CallingConvention = CallingConvention.Cdecl)]
        private static extern double CalculatePhe(double Qm, double Cp, double T1, double T2);
        [DllImport("COPandPCalc", CallingConvention = CallingConvention.Cdecl)]
        private static extern double CalculatePe(double U, double I);
        [DllImport("COPandPCalc", CallingConvention = CallingConvention.Cdecl)]
        private static extern double GetAirCp(double temperature);

        /// <summary>
        /// Calculates the COP (Coefficient of Performance)
        /// </summary>
        /// <param name="Phe">Phe Heat Capacity [W]</param>
        /// <param name="Pe">Pe Electrical Capacity [W]</param>
        /// <returns>The COP of the system [-]. <see cref="T:null" /> if DLL is not found.</returns>
        public static double? CalcCOP(double Phe, double Pe)
        {
            try
            {
                return CalculateCOP(Phe, Pe);
            }
            catch (DllNotFoundException dllNotFound)
            {
                ErrorPrinting.PrintError(dllNotFound.Message);
                return null;
            }
        }

        /// <summary>
        /// Calculates the Heat Capacity of given substance
        /// </summary>
        /// <param name="Qm">mass flow rate [kg/s]</param>
        /// <param name="Cp">specific thermal capacity of given substance [J/KgK]</param>
        /// <param name="DeltaT">temperature difference [K]</param>
        /// <returns>Heat Capacity [W]. <see cref="T:null" /> if DLL is not found.</returns>
        public static double? CalcPhe(double Qm, double Cp, double DeltaT)
        {
            try
            {
                return CalculatePhe(Qm, Cp, DeltaT);
            }
            catch (DllNotFoundException dllNotFoundException)
            {
                ErrorPrinting.PrintError(dllNotFoundException.Message);
                return null;
            }
        }

        /// <summary>
        /// Calculates the Heat Capacity of given substance
        /// </summary>
        /// <param name="Qm">mass flow rate [kg/s]</param>
        /// <param name="Cp">specific thermal capacity of given substance [J/KgK]</param>
        /// <param name="T1">temperature [T] or [°C]</param>
        /// <param name="T2">temperature [T] or [°C]</param>
        /// <returns>- Heat Capacity [W]. <see cref="T:null" /> if DLL is not found.</returns>
        public static double? CalcPhe(double Qm, double Cp, double T1, double T2)
        {
            try
            {
                return CalculatePhe(Qm, Cp, T1, T2);
            }
            catch (DllNotFoundException dllNotFoundException)
            {
                ErrorPrinting.PrintError(dllNotFoundException.Message);
                return null;
            }
        }

        /// <summary>
        /// Calculates the (consumed) Electrical capacity
        /// </summary>
        /// <param name="U">Voltage</param>
        /// <param name="I">Current</param>
        /// <returns>Electrical capacity. <see cref="T:null" /> if DLL is not found.</returns>
        public static double? CalcPe(double U, double I)
        {
            try
            {
                return CalculatePe(U, I);

            }
            catch (DllNotFoundException dllNotFound)
            {
                ErrorPrinting.PrintError(dllNotFound.Message);
                return null;
            }
        }

        /// <summary>
        /// Calculates the specific heat of air.
        /// </summary>
        /// <param name="temperature">ambient temperature</param>
        /// <returns>specific heat capacity [J]</returns>
        public static double? CalculateAirCp(double temperature)
        {
            try
            {
                return GetAirCp(temperature);
            }
            catch (DllNotFoundException dllNotFound)
            {
                ErrorPrinting.PrintError(dllNotFound.Message);
                throw;
            }
        }
    }
}
