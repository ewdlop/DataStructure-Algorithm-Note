#pragma once
#include <memory>
#include <memory_resource>
#include <array>
#include <vector>

class MemoryManager {
public:
    static constexpr size_t FRAME_COUNT = 3;
    static constexpr size_t FRAME_MEMORY = 1024 * 1024; // 1MB per frame
    static constexpr size_t PERSISTENT_MEMORY = 16 * 1024 * 1024; // 16MB
    static constexpr size_t UPLOAD_MEMORY = 8 * 1024 * 1024; // 8MB

    struct MemoryStats {
        size_t frameMemoryUsed;
        size_t persistentMemoryUsed;
        size_t uploadMemoryUsed;
        size_t peakFrameMemory;
        size_t peakPersistentMemory;
        size_t peakUploadMemory;
    };

    MemoryManager() {
        // Initialize memory pools
        m_persistentMemory = std::make_unique<std::byte[]>(PERSISTENT_MEMORY);
        m_persistentPool = std::pmr::monotonic_buffer_resource(
            m_persistentMemory.get(),
            PERSISTENT_MEMORY
        );

        m_uploadMemory = std::make_unique<std::byte[]>(UPLOAD_MEMORY);
        m_uploadPool = std::pmr::monotonic_buffer_resource(
            m_uploadMemory.get(),
            UPLOAD_MEMORY
        );

        for (auto& frameMemory : m_frameMemory) {
            frameMemory = std::make_unique<std::byte[]>(FRAME_MEMORY);
        }

        for (auto& framePool : m_framePools) {
            framePool = std::pmr::monotonic_buffer_resource(
                m_frameMemory[&framePool - &m_framePools[0]].get(),
                FRAME_MEMORY
            );
        }
    }

    // Frame-based allocator for temporary data
    template<typename T>
    T* FrameAlloc(size_t count = 1) {
        size_t size = sizeof(T) * count;
        return static_cast<T*>(
            m_framePools[m_currentFrame].allocate(
                size,
                std::alignment_of<T>::value
            )
        );
    }

    // Persistent allocator for long-lived data
    template<typename T>
    T* PersistentAlloc(size_t count = 1) {
        size_t size = sizeof(T) * count;
        return static_cast<T*>(
            m_persistentPool.allocate(
                size,
                std::alignment_of<T>::value
            )
        );
    }

    // Upload allocator for staging buffers
    template<typename T>
    T* UploadAlloc(size_t count = 1) {
        size_t size = sizeof(T) * count;
        return static_cast<T*>(
            m_uploadPool.allocate(
                size,
                std::alignment_of<T>::value
            )
        );
    }

    // STL-compatible allocators
    template<typename T>
    class FrameAllocator {
    public:
        using value_type = T;

        FrameAllocator(MemoryManager& manager) : m_manager(manager) {}

        template<typename U>
        FrameAllocator(const FrameAllocator<U>& other) 
            : m_manager(other.m_manager) {}

        T* allocate(size_t n) {
            return m_manager.FrameAlloc<T>(n);
        }

        void deallocate(T* p, size_t n) {
            // No-op for monotonic allocator
        }

    private:
        MemoryManager& m_manager;
    };

    void BeginFrame() {
        m_currentFrame = (m_currentFrame + 1) % FRAME_COUNT;
        m_framePools[m_currentFrame].release();
        
        // Reset upload buffer if needed
        if (m_uploadPool.buffer_size() > UPLOAD_MEMORY / 2) {
            m_uploadPool.release();
        }

        UpdateStats();
    }

    const MemoryStats& GetStats() const {
        return m_stats;
    }

private:
    std::unique_ptr<std::byte[]> m_persistentMemory;
    std::pmr::monotonic_buffer_resource m_persistentPool;

    std::unique_ptr<std::byte[]> m_uploadMemory;
    std::pmr::monotonic_buffer_resource m_uploadPool;

    std::array<std::unique_ptr<std::byte[]>, FRAME_COUNT> m_frameMemory;
    std::array<std::pmr::monotonic_buffer_resource, FRAME_COUNT> m_framePools;

    uint32_t m_currentFrame = 0;
    MemoryStats m_stats = {};

    void UpdateStats() {
        m_stats.frameMemoryUsed = m_framePools[m_currentFrame].buffer_size();
        m_stats.persistentMemoryUsed = m_persistentPool.buffer_size();
        m_stats.uploadMemoryUsed = m_uploadPool.buffer_size();

        m_stats.peakFrameMemory = std::max(
            m_stats.peakFrameMemory,
            m_stats.frameMemoryUsed
        );
        m_stats.peakPersistentMemory = std::max(
            m_stats.peakPersistentMemory,
            m_stats.persistentMemoryUsed
        );
        m_stats.peakUploadMemory = std::max(
            m_stats.peakUploadMemory,
            m_stats.uploadMemoryUsed
        );
    }
};