##
## Created by Juraj Lahvička
##

project(runTests)

#If tests very run in VS the linker was throwing an error because of the library was already defined but dll
set(gtest_force_shared_crt TRUE)

#Google tests
add_subdirectory(../tests/googletest-master ${CMAKE_CURRENT_SOURCE_DIR}/../tests/bin)
include_directories(../tests/googletest-master/googletest/include)
include_directories(../tests/googletest-master/googlemock/include)

set(GTEST_LIBRARY ../tests/googletest-master/googletest)
set(GTEST_INCLUDE_DIR ../tests/googletest-master/googletest/include)
set(GTEST_MAIN_LIBRARY ../tests/googletest-master/googletest)

enable_testing()
find_package(GTest REQUIRED)
find_package(Threads REQUIRED)

add_executable(${PROJECT_NAME}
        ../tests/library_tests.cpp
        ../src/library.cpp
        ../src/library.h
        ../tests/CpTests.cpp
        ../src/Cp.cpp
        ../src/Cp.h
        )

target_link_libraries(${PROJECT_NAME}
        gtest
        gtest_main
        )

add_test(library_test ${PROJECT_NAME})