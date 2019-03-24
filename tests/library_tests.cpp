//
// Created by Juraj Lahvička
//

#include "googletest-master/googletest/include/gtest/gtest.h"

#include "googletest-master/googlemock/include/gmock/gmock.h"
#include "../src/library.h"


TEST(CalculateCOP, Basic)
{
  ASSERT_DOUBLE_EQ(CalculateCOP(100, 20), 5);
}

TEST(CalculatePhe, DeltaT)
{
  ASSERT_DOUBLE_EQ(CalculatePhe(150, 1.005, 5), 753.75);
}

TEST(CalculatePhe, Temperatures)
{
  ASSERT_DOUBLE_EQ(CalculatePhe(150, 1.005, 20, 15), 753.75);
}

TEST(CalculatePe, Basic)
{
  ASSERT_DOUBLE_EQ(CalculatePe(230, 2), 460);
}
