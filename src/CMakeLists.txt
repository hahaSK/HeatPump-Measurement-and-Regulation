cmake_minimum_required(VERSION 3.11)
project(HeatPump_Measurement_and_Regulation)

set(CMAKE_CXX_STANDARD 11)

# create DLLs in gui/MainGUI/bin/Debug
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}/../gui/MainGUI/MainGUI/bin/Debug")

add_library(COPandPCalc SHARED library.cpp library.h Cp.cpp Cp.h)
