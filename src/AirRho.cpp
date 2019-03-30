//
// Created by Juraj Lahvička
//

#include <cmath>
#include "AirRho.h"

double GetAirRho(double h, double humidity, double airTemperature)
{
  const double T0{288.15}; // K
  const double L{6.5}; // K/km
  const double p0{101.325}; // kPa
  const double g{9.80665}; // m/s2
  const double Mair{0.0289644}; // kg/mol
  const double R{8.31447}; // J/mol K
  const double Mwater{0.018016}; // kg/mol

  const double T = 273.15 + airTemperature;

  double Th = T0 - (L / 1000) * h;

  double division = (g * Mair * h) / (R * Th);

  double p = (p0 * 1000) * exp(-division);

  if (humidity < 1) return (p * Mair) / (R * T);
  else
  {
    double psat = 6.1078 * pow(10, ((7.5 * airTemperature) / (237.3 + airTemperature)));
    double pv = (humidity / 100) * psat * 100;
    double pd = p - pv;
    return (pd * Mair + pv * Mwater) / (R * T);
  }
}