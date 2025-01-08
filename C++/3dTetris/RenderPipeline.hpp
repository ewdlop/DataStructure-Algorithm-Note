#pragma once
#include <DirectXMath.h>
#include <vector>
#include <array>
#include <span>
#include <memory_resource>

class RenderPipeline {
public:
    // Structured buffer for instance data
    struct InstanceData {
        XMFLOAT4X4 world;
        XMFLOAT4 color;
        XMFLOAT4 userData;
    };

    // Command sorting and batching
    struct RenderCommand {
        uint64_t sortKey;
        uint32_t meshId;
        uint32_t materialId;
        uint32_t instanceOffset;
        uint32_t instanceCount;
    };

    static constexpr size_t MAX_INSTANCES_PER_BATCH = 1024;
    static constexpr size_t FRAME_COUNT = 3; // Triple buffering

    RenderPipeline(ID3D11Device* device, ID3D11DeviceContext* context)
        : m_device(device)
        , m_context(context)
        , m_currentFrame(0) {
        
        CreateBuffers();
    }

    void BeginFrame() {
        m_currentFrame = (m_currentFrame + 1) % FRAME_COUNT;
        m_commands[m_currentFrame].clear();
        m_instanceData[m_currentFrame].clear();
        
        // Reset memory pools
        m_frameAllocator[m_currentFrame].release();
    }

    template<typename T>
    void Submit(const MeshView<T>& mesh,
               const Material& material,
               std::span<const InstanceData> instances) {
        
        // Split into batches if needed
        for (size_t offset = 0; offset < instances.size(); 
             offset += MAX_INSTANCES_PER_BATCH) {
            
            size_t batchSize = std::min(
                MAX_INSTANCES_PER_BATCH,
                instances.size() - offset
            );

            // Add instances to buffer
            size_t instanceOffset = m_instanceData[m_currentFrame].size();
            m_instanceData[m_currentFrame].insert(
                m_instanceData[m_currentFrame].end(),
                instances.begin() + offset,
                instances.begin() + offset + batchSize
            );

            // Create render command
            uint64_t sortKey = CalculateSortKey(material, mesh);
            m_commands[m_currentFrame].push_back({
                sortKey,
                mesh.GetId(),
                material.GetId(),
                static_cast<uint32_t>(instanceOffset),
                static_cast<uint32_t>(batchSize)
            });
        }
    }

    void EndFrame() {
        // Sort commands for optimal rendering
        std::sort(m_commands[m_currentFrame].begin(),
                 m_commands[m_currentFrame].end(),
                 [](const RenderCommand& a, const RenderCommand& b) {
                     return a.sortKey < b.sortKey;
                 });

        // Update instance buffer
        D3D11_MAPPED_SUBRESOURCE mapped;
        m_context->Map(m_instanceBuffers[m_currentFrame].Get(), 
                      0, D3D11_MAP_WRITE_DISCARD, 0, &mapped);
        
        memcpy(mapped.pData,
               m_instanceData[m_currentFrame].data(),
               m_instanceData[m_currentFrame].size() * sizeof(InstanceData));
        
        m_context->Unmap(m_instanceBuffers[m_currentFrame].Get(), 0);

        // Execute commands
        uint32_t currentMesh = ~0u;
        uint32_t currentMaterial = ~0u;

        for (const auto& cmd : m_commands[m_currentFrame]) {
            // Bind mesh if changed
            if (cmd.meshId != currentMesh) {
                BindMesh(cmd.meshId);
                currentMesh = cmd.meshId;
            }

            // Bind material if changed
            if (cmd.materialId != currentMaterial) {
                BindMaterial(cmd.materialId);
                currentMaterial = cmd.materialId;
            }

            // Draw instances
            m_context->DrawIndexedInstanced(
                GetMeshIndexCount(cmd.meshId),
                cmd.instanceCount,
                0,
                0,
                cmd.instanceOffset
            );
        }
    }

private:
    ID3D11Device* m_device;
    ID3D11DeviceContext* m_context;
    uint32_t m_currentFrame;

    // Per-frame resources
    std::array<ComPtr<ID3D11Buffer>, FRAME_COUNT> m_instanceBuffers;
    std::array<std::vector<RenderCommand>, FRAME_COUNT> m_commands;
    std::array<std::vector<InstanceData>, FRAME_COUNT> m_instanceData;
    std::array<std::pmr::monotonic_buffer_resource, FRAME_COUNT> m_frameAllocator;

    void CreateBuffers() {
        D3D11_BUFFER_DESC desc = {};
        desc.Usage = D3D11_USAGE_DYNAMIC;
        desc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
        desc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
        desc.MiscFlags = 0;
        desc.StructureByteStride = sizeof(InstanceData);
        desc.ByteWidth = sizeof(InstanceData) * MAX_INSTANCES_PER_BATCH;

        for (uint32_t i = 0; i < FRAME_COUNT; ++i) {
            m_device->CreateBuffer(&desc, nullptr, &m_instanceBuffers[i]);
            
            // Initialize frame allocators
            static constexpr size_t FRAME_MEMORY = 1024 * 1024; // 1MB per frame
            m_frameAllocator[i] = std::pmr::monotonic_buffer_resource(
                FRAME_MEMORY
            );
        }
    }

    uint64_t CalculateSortKey(const Material& material, const MeshView<>& mesh) {
        // Optimal sort key for minimizing state changes:
        // | PSO (16) | Shader (16) | Material (16) | Mesh (16) |
        uint64_t key = 0;
        key |= uint64_t(material.GetPipelineId()) << 48;
        key |= uint64_t(material.GetShaderId()) << 32;
        key |= uint64_t(material.GetId()) << 16;
        key |= uint64_t(mesh.GetId());
        return key;
    }
};