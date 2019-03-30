#ifndef HEATPUMP_MEASUREMENT_AND_REGULATION_LIBRARY_H
#define HEATPUMP_MEASUREMENT_AND_REGULATION_LIBRARY_H

#define DllEXPORT extern "C" __declspec(dllexport)

/**
 * Calculates the COP (Coefficient of Performance)
 * @param Phe Heat Capacity [W]
 * @param Pe Electrical Capacity [W]
 * @return The COP of the system [-]
 */
DllEXPORT double CalculateCOP(double Phe, double Pe);

/**
 * Calculates the Heat Capacity of given substance
 * @param Qm mass flow rate [kg/s]
 * @param Cp thermal capacity of given substance [J/KgK]
 * @param DeltaT temperature difference [K]
 * @return <strong>Phe</strong> - Heat Capacity [W]
 */
DllEXPORT double CalculatePhe(double Qm, double Cp, double DeltaT);

/**
 * Calculates the Heat Capacity ofc given substance
 * @param Qm mass flow rate [kg/s]
 * @param Cp thermal capacity of given substance [J/KgK]
 * @param T1 temperature [T] or [°C]
 * @param T2 temperature [T] or [°C]
 * @return <strong>Phe</strong> - Heat Capacity [W]
 */
__declspec(dllexport) double CalculatePhe(double Qm, double Cp, double T1, double T2);

/**
 * Calculates the (consumed) Electrical capacity
 * @param U Voltage
 * @param I Current
 * @return Electrical Capacity
 */
DllEXPORT double CalculatePe(double U, double I);

#endif