//
// Created by Juraj Lahvička
//

#include "googletest-master/googletest/include/gtest/gtest.h"

#include "googletest-master/googlemock/include/gmock/gmock.h"
#include "../src/Cp.h"

TEST(GetAirCp, Basic)
{
  ASSERT_DOUBLE_EQ(GetAirCp(20), 1004.6575);
  ASSERT_DOUBLE_EQ(GetAirCp(0), 1003.6575);
}

