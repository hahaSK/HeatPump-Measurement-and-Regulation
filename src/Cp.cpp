//
// Created by Juraj Lahvička
//

#include "Cp.h"

double GetAirCp(double temp)
{
  // for more info see section 6.1 of my thesis.
  double resultkJ = (273.15 + temp - 300) * 0.00005 + 1.005;
  return resultkJ * 1000;
}