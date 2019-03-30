//
// Created by Juraj Lahvička
//

#include "googletest-master/googletest/include/gtest/gtest.h"

#include "googletest-master/googlemock/include/gmock/gmock.h"
#include "../src/AirRho.h"

TEST(GetAirRho, Basic)
{
  ASSERT_DOUBLE_EQ(GetAirRho(0, 0, 15), 1.224977);
  ASSERT_DOUBLE_EQ(GetAirRho(150, 32, 40), 1.128069);
  ASSERT_DOUBLE_EQ(GetAirRho(0, 35, 20), 1.200407);
}