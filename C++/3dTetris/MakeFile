cmake_minimum_required(VERSION 3.20)
project(Tetris3D VERSION 1.0.0)

# C++ Standard
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

# Output directories
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)

# Options
option(BUILD_SHARED_LIBS "Build shared libraries" OFF)
option(ENABLE_TESTING "Enable testing" OFF)
option(ENABLE_PROFILING "Enable profiling" OFF)

# Dependencies
find_package(DirectX REQUIRED)
find_package(XAudio2 REQUIRED)
find_package(DirectXMath REQUIRED)

# Source files
set(SOURCES
    src/main.cpp
    src/Game.cpp
    src/Graphics.cpp
    src/Audio.cpp
    src/Input.cpp
    src/UI.cpp
    src/ParticleSystem.cpp
    src/Camera.cpp
    src/DebugRenderer.cpp
    src/ProfilerSystem.cpp
)

# Header files
set(HEADERS
    include/Game.h
    include/Graphics.h
    include/Audio.h
    include/Input.h
    include/UI.h
    include/ParticleSystem.h
    include/Camera.h
    include/DebugRenderer.h
    include/ProfilerSystem.h
)

# Shader files
set(SHADERS
    shaders/VertexShader.hlsl
    shaders/PixelShader.hlsl
    shaders/ParticleShader.hlsl
)

# Create executable
add_executable(${PROJECT_NAME} ${SOURCES} ${HEADERS})

# Include directories
target_include_directories(${PROJECT_NAME}
    PRIVATE
        ${CMAKE_CURRENT_SOURCE_DIR}/include
        ${DirectX_INCLUDE_DIRS}
)

# Link libraries
target_link_libraries(${PROJECT_NAME}
    PRIVATE
        d3d11
        dxgi
        d3dcompiler
        XAudio2
        DirectXMath
)

# Compile definitions
target_compile_definitions(${PROJECT_NAME}
    PRIVATE
        $<$<CONFIG:Debug>:_DEBUG>
        $<$<CONFIG:Release>:NDEBUG>
        UNICODE
        _UNICODE
)

# Compile options
target_compile_options(${PROJECT_NAME}
    PRIVATE
        $<$<CXX_COMPILER_ID:MSVC>:/W4 /WX>
        $<$<NOT:$<CXX_COMPILER_ID:MSVC>>:-Wall -Wextra -Wpedantic -Werror>
)

# Shader compilation
find_program(FXC fxc HINTS "C:/Program Files (x86)/Windows Kits/10/bin/${CMAKE_VS_WINDOWS_TARGET_PLATFORM_VERSION}/x64")
if(FXC)
    foreach(SHADER ${SHADERS})
        get_filename_component(SHADER_NAME ${SHADER} NAME_WE)
        get_filename_component(SHADER_EXT ${SHADER} EXT)
        
        if(${SHADER_EXT} STREQUAL ".hlsl")
            if(${SHADER_NAME} MATCHES ".*VS$")
                set(SHADER_TYPE vs_5_0)
            elseif(${SHADER_NAME} MATCHES ".*PS$")
                set(SHADER_TYPE ps_5_0)
            endif()
            
            add_custom_command(
                OUTPUT ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/shaders/${SHADER_NAME}.cso
                COMMAND ${FXC} /T ${SHADER_TYPE} /Fo ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/shaders/${SHADER_NAME}.cso ${CMAKE_CURRENT_SOURCE_DIR}/${SHADER}
                DEPENDS ${SHADER}
                COMMENT "Compiling ${SHADER}"
            )
            list(APPEND SHADER_OUTPUTS ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/shaders/${SHADER_NAME}.cso)
        endif()
    endforeach()

    add_custom_target(shaders ALL DEPENDS ${SHADER_OUTPUTS})
    add_dependencies(${PROJECT_NAME} shaders)
endif()

# Installation
install(TARGETS ${PROJECT_NAME}
    RUNTIME DESTINATION bin
    LIBRARY DESTINATION lib
    ARCHIVE DESTINATION lib
)

install(DIRECTORY ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/shaders
    DESTINATION bin
)

install(DIRECTORY assets
    DESTINATION bin
)

# Testing
if(ENABLE_TESTING)
    enable_testing()
    add_subdirectory(tests)
endif()

# Packaging
set(CPACK_PACKAGE_NAME ${PROJECT_NAME})
set(CPACK_PACKAGE_VERSION ${PROJECT_VERSION})
set(CPACK_PACKAGE_VENDOR "Your Company")
set(CPACK_PACKAGE_DESCRIPTION_SUMMARY "3D Tetris Game")
set(CPACK_RESOURCE_FILE_LICENSE "${CMAKE_CURRENT_SOURCE_DIR}/LICENSE")
set(CPACK_RESOURCE_FILE_README "${CMAKE_CURRENT_SOURCE_DIR}/README.md")

if(WIN32)
    set(CPACK_GENERATOR "NSIS")
    set(CPACK_NSIS_INSTALL_ROOT "$PROGRAMFILES64")
    set(CPACK_NSIS_ENABLE_UNINSTALL_BEFORE_INSTALL ON)
    set(CPACK_NSIS_MUI_ICON "${CMAKE_CURRENT_SOURCE_DIR}/assets/icon.ico")
    set(CPACK_NSIS_MUI_UNIICON "${CMAKE_CURRENT_SOURCE_DIR}/assets/icon.ico")
endif()

include(CPack)

# IDE specific settings
set_property(DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR} PROPERTY VS_STARTUP_PROJECT ${PROJECT_NAME})

if(MSVC)
    set_target_properties(${PROJECT_NAME} PROPERTIES
        VS_DEBUGGER_WORKING_DIRECTORY "${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/$<CONFIG>"
    )
endif()