#pragma once
#include <d3d11.h>
#include <d3dcompiler.h>
#include <wrl/client.h>
#include <array>
#include <string_view>
#include <vector>
#include <unordered_map>
#include <memory>
#include <span>

using Microsoft::WRL::ComPtr;

class ShaderSystem {
public:
    // Shader types
    enum class ShaderType {
        Vertex,
        Pixel,
        Geometry,
        Compute
    };

    // Shader configuration
    struct ShaderConfig {
        static constexpr std::array<D3D11_INPUT_ELEMENT_DESC, 4> DEFAULT_INPUT_LAYOUT = {{
            {"POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0},
            {"NORMAL",   0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0},
            {"COLOR",    0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 24, D3D11_INPUT_PER_VERTEX_DATA, 0},
            {"TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 40, D3D11_INPUT_PER_VERTEX_DATA, 0}
        }};

        static constexpr std::string_view SHADER_ENTRY_POINT = "main";
        static constexpr std::array<std::string_view, 4> SHADER_MODELS = {
            "vs_5_0", "ps_5_0", "gs_5_0", "cs_5_0"
        };
    };

    // Shader resources
    struct ShaderResources {
        ComPtr<ID3D11VertexShader> vertexShader;
        ComPtr<ID3D11PixelShader> pixelShader;
        ComPtr<ID3D11GeometryShader> geometryShader;
        ComPtr<ID3D11ComputeShader> computeShader;
        ComPtr<ID3D11InputLayout> inputLayout;
        std::vector<ComPtr<ID3D11Buffer>> constantBuffers;
    };

private:
    // Shader source code storage
    struct ShaderSource {
        std::vector<uint8_t> bytecode;
        std::string entryPoint;
        std::string target;
    };

    ComPtr<ID3D11Device> m_device;
    std::unordered_map<std::string, ShaderResources> m_shaders;
    std::unordered_map<std::string, ShaderSource> m_shaderSources;

public:
    explicit ShaderSystem(ComPtr<ID3D11Device> device) : m_device(device) {}

    template<size_t N>
    bool CreateShaderProgram(
        const std::string& name,
        const std::array<std::pair<ShaderType, std::string_view>, N>& shaderSources,
        std::span<const D3D11_INPUT_ELEMENT_DESC> inputLayout = ShaderConfig::DEFAULT_INPUT_LAYOUT) {
        
        ShaderResources resources;
        
        for (const auto& [type, source] : shaderSources) {
            if (!CompileAndCreateShader(type, source, name, resources)) {
                return false;
            }
        }

        // Create input layout if vertex shader exists
        if (resources.vertexShader) {
            const auto& vsSource = m_shaderSources[name + "_vs"];
            if (!CreateInputLayout(vsSource.bytecode, inputLayout, resources.inputLayout)) {
                return false;
            }
        }

        m_shaders[name] = std::move(resources);
        return true;
    }

    const ShaderResources* GetShaderProgram(const std::string& name) const {
        auto it = m_shaders.find(name);
        return it != m_shaders.end() ? &it->second : nullptr;
    }

    bool CreateConstantBuffer(const std::string& shaderName, 
                            uint32_t size, 
                            uint32_t slot) {
        auto it = m_shaders.find(shaderName);
        if (it == m_shaders.end()) return false;

        D3D11_BUFFER_DESC desc = {};
        desc.ByteWidth = size;
        desc.Usage = D3D11_USAGE_DYNAMIC;
        desc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
        desc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;

        ComPtr<ID3D11Buffer> buffer;
        if (FAILED(m_device->CreateBuffer(&desc, nullptr, &buffer))) {
            return false;
        }

        if (slot >= it->second.constantBuffers.size()) {
            it->second.constantBuffers.resize(slot + 1);
        }
        it->second.constantBuffers[slot] = buffer;
        return true;
    }

private:
    bool CompileAndCreateShader(
        ShaderType type,
        std::string_view source,
        const std::string& name,
        ShaderResources& resources) {
        
        // Determine shader model
        const auto& shaderModel = ShaderConfig::SHADER_MODELS[static_cast<size_t>(type)];
        std::string suffix;
        
        switch (type) {
            case ShaderType::Vertex:   suffix = "_vs"; break;
            case ShaderType::Pixel:    suffix = "_ps"; break;
            case ShaderType::Geometry: suffix = "_gs"; break;
            case ShaderType::Compute:  suffix = "_cs"; break;
        }

        ComPtr<ID3DBlob> shaderBlob;
        ComPtr<ID3DBlob> errorBlob;

        // Compile shader
        HRESULT hr = D3DCompile(
            source.data(),
            source.size(),
            nullptr,
            nullptr,
            D3D_COMPILE_STANDARD_FILE_INCLUDE,
            std::string(ShaderConfig::SHADER_ENTRY_POINT).c_str(),
            std::string(shaderModel).c_str(),
            D3DCOMPILE_ENABLE_STRICTNESS | D3DCOMPILE_DEBUG,
            0,
            &shaderBlob,
            &errorBlob
        );

        if (FAILED(hr)) {
            if (errorBlob) {
                OutputDebugStringA(static_cast<char*>(errorBlob->GetBufferPointer()));
            }
            return false;
        }

        // Store shader bytecode
        ShaderSource shaderSource;
        shaderSource.bytecode.resize(shaderBlob->GetBufferSize());
        std::memcpy(shaderSource.bytecode.data(), shaderBlob->GetBufferPointer(), shaderBlob->GetBufferSize());
        shaderSource.entryPoint = std::string(ShaderConfig::SHADER_ENTRY_POINT);
        shaderSource.target = std::string(shaderModel);
        m_shaderSources[name + suffix] = std::move(shaderSource);

        // Create shader
        switch (type) {
            case ShaderType::Vertex:
                hr = m_device->CreateVertexShader(
                    shaderBlob->GetBufferPointer(),
                    shaderBlob->GetBufferSize(),
                    nullptr,
                    &resources.vertexShader
                );
                break;

            case ShaderType::Pixel:
                hr = m_device->CreatePixelShader(
                    shaderBlob->GetBufferPointer(),
                    shaderBlob->GetBufferSize(),
                    nullptr,
                    &resources.pixelShader
                );
                break;

            case ShaderType::Geometry:
                hr = m_device->CreateGeometryShader(
                    shaderBlob->GetBufferPointer(),
                    shaderBlob->GetBufferSize(),
                    nullptr,
                    &resources.geometryShader
                );
                break;

            case ShaderType::Compute:
                hr = m_device->CreateComputeShader(
                    shaderBlob->GetBufferPointer(),
                    shaderBlob->GetBufferSize(),
                    nullptr,
                    &resources.computeShader
                );
                break;
        }

        return SUCCEEDED(hr);
    }

    bool CreateInputLayout(
        const std::vector<uint8_t>& vertexShaderBytecode,
        std::span<const D3D11_INPUT_ELEMENT_DESC> inputElements,
        ComPtr<ID3D11InputLayout>& inputLayout) {
        
        return SUCCEEDED(m_device->CreateInputLayout(
            inputElements.data(),
            static_cast<UINT>(inputElements.size()),
            vertexShaderBytecode.data(),
            vertexShaderBytecode.size(),
            &inputLayout
        ));
    }
};