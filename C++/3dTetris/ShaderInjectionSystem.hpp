#pragma once
#include <d3d11.h>
#include <d3dcompiler.h>
#include <string>
#include <unordered_map>
#include <memory>
#include <filesystem>
#include <functional>

class ShaderInjectionSystem {
public:
    // Shader modification callback type
    using ShaderModifierFn = std::function<std::string(const std::string&)>;

    struct InjectionPoint {
        std::string marker;          // Marker in shader code
        std::string injectedCode;    // Code to inject
        bool enabled = true;
    };

    struct ShaderVariant {
        std::string name;
        std::vector<std::string> defines;
        std::vector<InjectionPoint> injectionPoints;
        bool enabled = true;
    };

    ShaderInjectionSystem(ID3D11Device* device) : m_device(device) {
        // Register default injectors
        RegisterInjector("DEBUG", [](const std::string& code) {
            return "#define DEBUG 1\n" + code;
        });

        RegisterInjector("PROFILING", [](const std::string& code) {
            return "#define PROFILING 1\n"
                   "#define START_PROFILE(name) ProfileBlock profile##__LINE__(name)\n" +
                   code;
        });
    }

    // Register a new shader modifier
    void RegisterInjector(const std::string& name, ShaderModifierFn modifier) {
        m_modifiers[name] = modifier;
    }

    // Add injection point
    void AddInjectionPoint(const std::string& shaderName, const InjectionPoint& point) {
        m_injectionPoints[shaderName].push_back(point);
    }

    // Create shader variant
    void CreateVariant(const std::string& baseName, const ShaderVariant& variant) {
        m_variants[baseName + ":" + variant.name] = variant;
    }

    // Load and compile shader with injections
    ID3D11VertexShader* CompileVertexShader(
        const std::string& shaderPath,
        const std::string& variantName = "") {
        
        std::string processedCode = LoadAndProcessShader(shaderPath, variantName);
        
        // Compile shader
        ComPtr<ID3DBlob> shaderBlob;
        ComPtr<ID3DBlob> errorBlob;
        HRESULT hr = D3DCompile(
            processedCode.c_str(),
            processedCode.length(),
            shaderPath.c_str(),
            nullptr,
            D3D_COMPILE_STANDARD_FILE_INCLUDE,
            "main",
            "vs_5_0",
            D3DCOMPILE_DEBUG | D3DCOMPILE_SKIP_OPTIMIZATION,
            0,
            &shaderBlob,
            &errorBlob
        );

        if (FAILED(hr)) {
            if (errorBlob) {
                // Log error
                OutputDebugStringA(static_cast<char*>(errorBlob->GetBufferPointer()));
            }
            return nullptr;
        }

        // Create shader
        ID3D11VertexShader* shader;
        hr = m_device->CreateVertexShader(
            shaderBlob->GetBufferPointer(),
            shaderBlob->GetBufferSize(),
            nullptr,
            &shader
        );

        if (FAILED(hr)) {
            return nullptr;
        }

        return shader;
    }

    // Dynamic shader patching at runtime
    bool PatchShader(const std::string& shaderName, 
                    const std::string& markerName,
                    const std::string& newCode) {
        auto& points = m_injectionPoints[shaderName];
        for (auto& point : points) {
            if (point.marker == markerName) {
                point.injectedCode = newCode;
                return true;
            }
        }
        return false;
    }

    // Enable/disable injection points
    void SetInjectionPointEnabled(const std::string& shaderName,
                                const std::string& markerName,
                                bool enabled) {
        auto& points = m_injectionPoints[shaderName];
        for (auto& point : points) {
            if (point.marker == markerName) {
                point.enabled = enabled;
                break;
            }
        }
    }

private:
    ID3D11Device* m_device;
    std::unordered_map<std::string, ShaderModifierFn> m_modifiers;
    std::unordered_map<std::string, std::vector<InjectionPoint>> m_injectionPoints;
    std::unordered_map<std::string, ShaderVariant> m_variants;

    std::string LoadAndProcessShader(const std::string& path, 
                                   const std::string& variantName) {
        // Load shader file
        std::ifstream file(path);
        if (!file.is_open()) {
            return "";
        }

        std::stringstream buffer;
        buffer << file.rdbuf();
        std::string code = buffer.str();

        // Apply variant defines if specified
        if (!variantName.empty()) {
            if (auto it = m_variants.find(variantName); it != m_variants.end()) {
                for (const auto& define : it->second.defines) {
                    code = "#define " + define + "\n" + code;
                }
            }
        }

        // Apply active injections
        auto& points = m_injectionPoints[path];
        for (const auto& point : points) {
            if (point.enabled) {
                size_t pos = code.find(point.marker);
                if (pos != std::string::npos) {
                    code.insert(pos + point.marker.length(), "\n" + point.injectedCode);
                }
            }
        }

        // Apply modifiers
        for (const auto& [name, modifier] : m_modifiers) {
            code = modifier(code);
        }

        return code;
    }
};