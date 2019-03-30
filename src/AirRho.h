//
// Created by Juraj Lahvička
//

#ifndef HEATPUMP_MEASUREMENT_AND_REGULATION_AIRRHO_H
#define HEATPUMP_MEASUREMENT_AND_REGULATION_AIRRHO_H

#define DllEXPORT extern "C" __declspec(dllexport)

/**
 * Calculate air density.
 * @param h height above sea level [m]
 * @param humidity percentage of humidity [%]
 * @param airTemperature air temperature [°C]
 * @return air density [kg/m3]
 */
DllEXPORT double GetAirRho(double h, double humidity, double airTemperature);

#endif //HEATPUMP_MEASUREMENT_AND_REGULATION_AIRRHO_H
