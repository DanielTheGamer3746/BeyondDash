cmake_minimum_required(VERSION 3.21)

# Project definition
project(BeyondDash VERSION 1.0.0)

set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_VISIBILITY_PRESET hidden)

# Architecture settings
if ("${CMAKE_SYSTEM_NAME}" STREQUAL "iOS" OR IOS)
    set(CMAKE_OSX_ARCHITECTURES "arm64")
else()
    set(CMAKE_OSX_ARCHITECTURES "arm64;x86_64")
endif()

# 1. Locate Geode SDK
if (NOT DEFINED ENV{GEODE_SDK})
    message(FATAL_ERROR "Unable to find Geode SDK! Please define GEODE_SDK environment variable to point to Geode")
else()
    message(STATUS "Found Geode: $ENV{GEODE_SDK}")
endif()

# 2. Add Geode Subdirectory
add_subdirectory($ENV{GEODE_SDK} ${CMAKE_CURRENT_BINARY_DIR}/geode)

# 3. Gather Source Files
# Note: You had set sources manually and then globbed them. 
# I kept the GLOB_RECURSE as it covers everything in src/.
file(GLOB_RECURSE SOURCES CONFIGURE_DEPENDS src/*.cpp)

# 4. Create the Library Target (This was the missing step!)
# Geode mods must be SHARED libraries.
add_library(${PROJECT_NAME} SHARED ${SOURCES})

# 5. Setup Geode Mod
# This applies dependencies and linking to the target created in step 4.
setup_geode_mod(${PROJECT_NAME})