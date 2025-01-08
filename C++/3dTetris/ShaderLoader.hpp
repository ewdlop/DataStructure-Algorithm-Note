#pragma once
#include <optional>
#include <variant>
#include <fstream>
#include <sstream>
#include <filesystem>

class ShaderLoader {
public:
    struct ShaderPreprocessor {
        static constexpr std::array<std::string_view, 2> INCLUDE_DIRECTIVES = {
            "#include",
            "#pragma include"
        };

        struct IncludeHandler {
            std::filesystem::path basePath;
            std::unordered_set<std::string> includedFiles;
            
            std::optional<std::string> ResolveInclude(
                const std::string& includePath,
                const std::filesystem::path& currentFile) {
                
                auto fullPath = ResolvePath(includePath, currentFile);
                if (includedFiles.contains(fullPath.string())) {
                    return std::nullopt; // Already included
                }
                
                includedFiles.insert(fullPath.string());
                return LoadFile(fullPath);
            }

            std::filesystem::path ResolvePath(
                const std::string& includePath,
                const std::filesystem::path& currentFile) {
                if (includePath[0] == '/') {
                    return basePath / includePath.substr(1);
                }
                return currentFile.parent_path() / includePath;
            }
        };

        static std::string PreprocessShader(
            const std::string& source,
            const std::filesystem::path& sourcePath,
            IncludeHandler& includeHandler) {
            
            std::stringstream output;
            std::stringstream input(source);
            std::string line;

            while (std::getline(input, line)) {
                bool isInclude = false;
                std::string includePath;

                for (const auto& directive : INCLUDE_DIRECTIVES) {
                    if (line.starts_with(directive)) {
                        isInclude = true;
                        includePath = ExtractIncludePath(line);
                        break;
                    }
                }

                if (isInclude && !includePath.empty()) {
                    if (auto included = includeHandler.ResolveInclude(
                            includePath, sourcePath)) {
                        output << *included << '\n';
                    }
                } else {
                    output << line << '\n';
                }
            }

            return output.str();
        }

    private:
        static std::string ExtractIncludePath(const std::string& line) {
            size_t start = line.find('"');
            if (start == std::string::npos) return "";
            
            size_t end = line.find('"', start + 1);
            if (end == std::string::npos) return "";
            
            return line.substr(start + 1, end - start - 1);
        }
    };

    struct ShaderCache {
        struct CacheEntry {
            std::vector<uint8_t> bytecode;
            std::filesystem::file_time_type sourceTime;
            FeatureBits features;
        };

        std::unordered_map<std::string, CacheEntry> entries;
        std::filesystem::path cachePath;

        void Load() {
            // Load cached shader bytecode
        }

        void Save() {
            // Save shader cache to disk
        }

        std::optional<std::vector<uint8_t>> GetCachedBytecode(
            const std::string& key,
            const std::filesystem::file_time_type& sourceTime,
            const FeatureBits& features) {
            
            auto it = entries.find(key);
            if (it != entries.end() &&
                it->second.sourceTime == sourceTime &&
                it->second.features == features) {
                return it->second.bytecode;
            }
            return std::nullopt;
        }

        void UpdateCache(
            const std::string& key,
            const std::vector<uint8_t>& bytecode,
            const std::filesystem::file_time_type& sourceTime,
            const FeatureBits& features) {
            
            entries[key] = CacheEntry{
                bytecode,
                sourceTime,
                features
            };
        }
    };

    struct CompileOptions {
        bool enableDebug = true;
        bool enableOptimization = true;
        bool enableStrictness = true;
        bool enableWarnings = true;
        std::vector<std::string> additionalFlags;

        [[nodiscard]]
        UINT GetCompileFlags() const {
            UINT flags = 0;
            if (enableDebug) flags |= D3DCOMPILE_DEBUG;
            if (enableOptimization) flags |= D3DCOMPILE_OPTIMIZATION_LEVEL3;
            if (enableStrictness) flags |= D3DCOMPILE_ENABLE_STRICTNESS;
            if (enableWarnings) flags |= D3DCOMPILE_WARNINGS_ARE_ERRORS;
            return flags;
        }
    };

    static std::variant<std::vector<uint8_t>, std::string> 
    LoadAndCompileShader(
        const std::filesystem::path& path,
        const ShaderVariant& variant,
        const CompileOptions& options = {}) {
        
        try {
            // Load source
            auto source = LoadFile(path);
            if (!source) {
                return "Failed to load shader file";
            }

            // Preprocess includes
            IncludeHandler includeHandler{path.parent_path()};
            auto preprocessed = PreprocessShader(*source, path, includeHandler);

            // Apply permutation defines
            auto finalSource = preprocessed + variant.macros;

            // Compile
            ComPtr<ID3DBlob> shaderBlob;
            ComPtr<ID3DBlob> errorBlob;

            HRESULT hr = D3DCompile(
                finalSource.c_str(),
                finalSource.length(),
                path.string().c_str(),
                nullptr,
                D3D_COMPILE_STANDARD_FILE_INCLUDE,
                "main",
                "ps_5_0",
                options.GetCompileFlags(),
                0,
                &shaderBlob,
                &errorBlob
            );

            if (FAILED(hr)) {
                if (errorBlob) {
                    return std::string(
                        static_cast<char*>(errorBlob->GetBufferPointer())
                    );
                }
                return "Shader compilation failed";
            }

            // Return bytecode
            std::vector<uint8_t> bytecode(
                static_cast<uint8_t*>(shaderBlob->GetBufferPointer()),
                static_cast<uint8_t*>(shaderBlob->GetBufferPointer()) + 
                    shaderBlob->GetBufferSize()
            );

            return bytecode;

        } catch (const std::exception& e) {
            return std::string(e.what());
        }
    }

private:
    static std::optional<std::string> LoadFile(
        const std::filesystem::path& path) {
        std::ifstream file(path);
        if (!file) return std::nullopt;

        std::stringstream buffer;
        buffer << file.rdbuf();
        return buffer.str();
    }
};