#ifndef HEATPUMP_MEASUREMENT_AND_REGULATION_CP_H
#define HEATPUMP_MEASUREMENT_AND_REGULATION_CP_H

#define DllEXPORT extern "C" __declspec(dllexport)

/**
 * Calculate specific heat capacity at given temperature.
 * @param temp ambient temperature
 * @return specific heat capacity of air in [J/kgK]
 */
DllEXPORT double GetAirCp(double temp);

#endif //HEATPUMP_MEASUREMENT_AND_REGULATION_CP_H
