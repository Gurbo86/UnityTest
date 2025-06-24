cmake_minimum_required(VERSION 3.10)
project(DeviceInfoPlugin)

set(CMAKE_CXX_STANDARD 17)

# Define the library
add_library(DeviceInfoPlugin SHARED
    src/DeviceInfoPlugin.cpp
)

# Platform-specific settings
if(WIN32)
    target_compile_definitions(DeviceInfoPlugin PRIVATE PLATFORM_WINDOWS)
elseif(ANDROID)
    target_compile_definitions(DeviceInfoPlugin PRIVATE PLATFORM_ANDROID)
    target_link_libraries(DeviceInfoPlugin PRIVATE log)
elseif(APPLE)
    target_compile_definitions(DeviceInfoPlugin PRIVATE PLATFORM_IOS)
endif()

# Add test executable only if not Android
if(NOT ANDROID)
    add_executable(DeviceInfoPluginTest src/DeviceInfoPluginTest.cpp)
    target_link_libraries(DeviceInfoPluginTest PRIVATE DeviceInfoPlugin)
endif()