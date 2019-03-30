//
// Created by Juraj Lahvička
//

#include "googletest-master/googletest/include/gtest/gtest.h"

#include "googletest-master/googlemock/include/gmock/gmock.h"
#include "../src/AirRho.h"

TEST(GetAirRho, Basic)
{
  float delta{0.000001};
  ASSERT_NEAR(GetAirRho(0, 0, 15), 1.22497705587732, delta);
  ASSERT_NEAR(GetAirRho(150, 40, 32), 1.12806909389719, delta);
  ASSERT_NEAR(GetAirRho(0, 35, 20), 1.20040787096485, delta);
  ASSERT_NEAR(GetAirRho(200, 80, 13), 1.19899065751873, delta);
}