//
// Created by Juraj Lahvička
//

#include "library.h"
#include <iostream>

double CalculateCOP(double Phe, double Pe)
{
  return Phe / Pe;
}

double CalculatePhe(double Qm, double Cp, double DeltaT)
{
  return Qm * Cp * DeltaT;
}

double CalculatePhe(double Qm, double Cp, double T1, double T2)
{
  return Qm * Cp * (T1 - T2);
}

double CalculatePe(double U, double I)
{
  return U * I;
}