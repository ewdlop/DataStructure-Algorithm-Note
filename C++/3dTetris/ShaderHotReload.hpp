#pragma once
#include <filesystem>
#include <chrono>
#include <thread>
#include <atomic>
#include <mutex>
#include <unordered_map>

class ShaderHotReload {
public:
    struct ShaderFile {
        std::filesystem::path path;
        std::filesystem::file_time_type lastWriteTime;
        std::string shaderName;
        ShaderType type;
    };

    ShaderHotReload(ShaderSystem& shaderSystem) 
        : m_shaderSystem(shaderSystem)
        , m_isWatching(false) {
    }

    ~ShaderHotReload() {
        StopWatching();
    }

    void WatchShader(const std::string& shaderName, 
                    const std::filesystem::path& filePath,
                    ShaderType type) {
        std::lock_guard<std::mutex> lock(m_mutex);
        
        ShaderFile file{
            filePath,
            std::filesystem::last_write_time(filePath),
            shaderName,
            type
        };
        
        m_watchedFiles[filePath.string()] = file;
    }

    void StartWatching() {
        if (m_isWatching) return;
        
        m_isWatching = true;
        m_watchThread = std::thread([this]() { WatcherThread(); });
    }

    void StopWatching() {
        m_isWatching = false;
        if (m_watchThread.joinable()) {
            m_watchThread.join();
        }
    }

private:
    ShaderSystem& m_shaderSystem;
    std::unordered_map<std::string, ShaderFile> m_watchedFiles;
    std::thread m_watchThread;
    std::atomic<bool> m_isWatching;
    std::mutex m_mutex;

    void WatcherThread() {
        while (m_isWatching) {
            {
                std::lock_guard<std::mutex> lock(m_mutex);
                for (auto& [path, file] : m_watchedFiles) {
                    try {
                        auto currentTime = std::filesystem::last_write_time(file.path);
                        if (currentTime != file.lastWriteTime) {
                            file.lastWriteTime = currentTime;
                            ReloadShader(file);
                        }
                    } catch (const std::filesystem::filesystem_error&) {
                        // Handle file access errors
                    }
                }
            }
            std::this_thread::sleep_for(std::chrono::milliseconds(100));
        }
    }

    void ReloadShader(const ShaderFile& file) {
        try {
            std::vector<uint8_t> shaderSource;
            if (LoadShaderSource(file.path, shaderSource)) {
                m_shaderSystem.ReloadShader(
                    file.shaderName,
                    file.type,
                    shaderSource
                );
            }
        } catch (const std::exception&) {
            // Handle shader reload errors
        }
    }

    bool LoadShaderSource(const std::filesystem::path& path,
                         std::vector<uint8_t>& outSource) {
        std::ifstream file(path, std::ios::binary | std::ios::ate);
        if (!file) return false;

        auto size = file.tellg();
        file.seekg(0);
        outSource.resize(size);
        return file.read(reinterpret_cast<char*>(outSource.data()), size).good();
    }
};