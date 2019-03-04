cmake_minimum_required(VERSION 3.11)
project(HeatPump_Measurement_and_Regulation)

set(CMAKE_CXX_STANDARD 11)

add_library(HeatPumpCalc SHARED library.cpp library.h)