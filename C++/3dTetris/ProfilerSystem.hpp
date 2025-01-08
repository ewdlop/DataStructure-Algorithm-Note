#pragma once
#include <chrono>
#include <string>
#include <array>
#include <unordered_map>
#include <vector>
#include <thread>
#include <mutex>

class ProfilerSystem {
public:
    struct ProfileConfig {
        static constexpr size_t MAX_FRAMES = 300;
        static constexpr size_t MAX_MARKERS = 1024;
        static constexpr size_t MAX_THREADS = 32;
        static constexpr float HISTORY_TIME = 5.0f; // 5 seconds of history
    };

    struct ProfileMarker {
        std::string name;
        std::chrono::high_resolution_clock::time_point start;
        std::chrono::high_resolution_clock::time_point end;
        uint32_t threadId;
        uint32_t parentIndex;
        uint32_t depth;
        bool isGPU;
    };

    struct ProfileFrame {
        std::vector<ProfileMarker> markers;
        float frameTime;
        uint64_t frameNumber;
    };

    struct ProfileStats {
        float minTime;
        float maxTime;
        float avgTime;
        float lastTime;
        uint32_t callCount;
    };

    class ScopedMarker {
    public:
        ScopedMarker(ProfilerSystem& profiler, const char* name, bool isGPU = false)
            : m_profiler(profiler), m_name(name), m_isGPU(isGPU) {
            m_profiler.BeginMarker(m_name, m_isGPU);
        }

        ~ScopedMarker() {
            m_profiler.EndMarker(m_name);
        }

    private:
        ProfilerSystem& m_profiler;
        const char* m_name;
        bool m_isGPU;
    };

    ProfilerSystem()
        : m_currentFrame(0)
        , m_frameCount(0)
        , m_enabled(true) {
        m_frames.resize(ProfileConfig::MAX_FRAMES);
    }

    void BeginFrame() {
        if (!m_enabled) return;

        m_frameStart = std::chrono::high_resolution_clock::now();
        m_currentFrame = (m_currentFrame + 1) % ProfileConfig::MAX_FRAMES;
        m_frames[m_currentFrame].markers.clear();
        m_frames[m_currentFrame].frameNumber = m_frameCount++;

        // Begin GPU frame query
        if (m_d3dQuery) {
            m_d3dContext->Begin(m_d3dQuery.Get());
        }
    }

    void EndFrame() {
        if (!m_enabled) return;

        auto frameEnd = std::chrono::high_resolution_clock::now();
        float frameTime = std::chrono::duration<float>(frameEnd - m_frameStart).count();
        m_frames[m_currentFrame].frameTime = frameTime;

        // End GPU frame query
        if (m_d3dQuery) {
            m_d3dContext->End(m_d3dQuery.Get());
            UpdateGPUTiming();
        }

        // Update statistics
        UpdateStats();
    }

    void BeginMarker(const char* name, bool isGPU = false) {
        if (!m_enabled) return;

        uint32_t threadId = static_cast<uint32_t>(
            std::hash<std::thread::id>{}(std::this_thread::get_id())
        );

        ProfileMarker marker;
        marker.name = name;
        marker.start = std::chrono::high_resolution_clock::now();
        marker.threadId = threadId;
        marker.isGPU = isGPU;
        marker.depth = GetCurrentDepth(threadId);

        // GPU timing
        if (isGPU) {
            BeginGPUMarker(name);
        }

        std::lock_guard<std::mutex> lock(m_mutex);
        m_frames[m_currentFrame].markers.push_back(marker);
    }

    void EndMarker(const char* name) {
        if (!m_enabled) return;

        std::lock_guard<std::mutex> lock(m_mutex);
        auto& markers = m_frames[m_currentFrame].markers;

        // Find matching marker
        for (auto it = markers.rbegin(); it != markers.rend(); ++it) {
            if (it->name == name && it->end.time_since_epoch().count() == 0) {
                it->end = std::chrono::high_resolution_clock::now();

                if (it->isGPU) {
                    EndGPUMarker();
                }
                break;
            }
        }
    }

    void RenderUI(DebugRenderer& debug) {
        if (!m_enabled) return;

        // Draw frame time graph
        DrawFrameTimeGraph(debug);

        // Draw marker hierarchy
        DrawMarkerHierarchy(debug);

        // Draw statistics
        DrawStats(debug);
    }

    // Helper macro for scoped profiling
    #define PROFILE_SCOPE(name) ProfilerSystem::ScopedMarker CONCAT(scopedMarker, __LINE__)(*this, name)
    #define PROFILE_SCOPE_GPU(name) ProfilerSystem::ScopedMarker CONCAT(scopedMarker, __LINE__)(*this, name, true)

private:
    std::vector<ProfileFrame> m_frames;
    size_t m_currentFrame;
    uint64_t m_frameCount;
    bool m_enabled;
    std::mutex m_mutex;
    std::chrono::high_resolution_clock::time_point m_frameStart;

    // GPU timing resources
    ComPtr<ID3D11DeviceContext> m_d3dContext;
    ComPtr<ID3D11Query> m_d3dQuery;
    std::vector<ComPtr<ID3D11Query>> m_gpuMarkerQueries;

    std::unordered_map<std::string, ProfileStats> m_stats;
    std::array<uint32_t, ProfileConfig::MAX_THREADS> m_threadDepths = {};

    void UpdateStats() {
        // Group markers by name and calculate
    }
}