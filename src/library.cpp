//
// Created by Juraj Lahvička
//

#include "library.h"
#include <iostream>

/**
 * Calculates the COP (Coefficient of Performance)
 * @param Phe Heat Capacity [W]
 * @param Pe Electrical Capacity [W]
 * @return The COP of the system [-]
 */
double CalculateCOP(double Phe, double Pe)
{
  return Phe / Pe;
}

/**
 * Calculates the Heat Capacity of given substance
 * @param Qm mass flow rate [kg/s]
 * @param Cp thermal capacity of given substance [J/KgK]
 * @param DeltaT temperature difference [K]
 * @return <strong>Phe</strong> - Heat Capacity [W]
 */
double CalculatePhe(double Qm, double Cp, double DeltaT)
{
  return Qm * Cp * DeltaT;
}

/**
 * Calculates the Heat Capacity ofc given substance
 * @param Qm mass flow rate [kg/s]
 * @param Cp thermal capacity of given substance [J/KgK]
 * @param T1 temperature [T] or [°C]
 * @param T2 temperature [T] or [°C]
 * @return <strong>Phe</strong> - Heat Capacity [W]
 */
double CalculatePhe(double Qm, double Cp, double T1, double T2)
{
  return Qm * Cp * (T1 - T2);
}