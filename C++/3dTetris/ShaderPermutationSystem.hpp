#pragma once
#include <string>
#include <vector>
#include <unordered_map>
#include <bitset>

class ShaderPermutationSystem {
public:
    static constexpr size_t MAX_FEATURES = 32;
    using FeatureBits = std::bitset<MAX_FEATURES>;

    struct ShaderFeature {
        std::string name;
        std::string define;
        uint32_t bit;
    };

    struct ShaderVariant {
        FeatureBits features;
        std::string macros;
        std::string uniqueId;
    };

    void RegisterFeature(const std::string& name, 
                        const std::string& define) {
        if (m_featureCount >= MAX_FEATURES) return;

        ShaderFeature feature{
            name,
            define,
            m_featureCount
        };
        
        m_features[name] = feature;
        m_featureCount++;
    }

    ShaderVariant CreateVariant(const std::vector<std::string>& enabledFeatures) {
        ShaderVariant variant;
        std::string macros;

        for (const auto& featureName : enabledFeatures) {
            if (auto it = m_features.find(featureName); 
                it != m_features.end()) {
                variant.features.set(it->second.bit);
                macros += "#define " + it->second.define + "\n";
            }
        }

        variant.macros = macros;
        variant.uniqueId = GenerateVariantId(variant.features);
        return variant;
    }

    template<typename... Features>
    ShaderVariant CreateVariant(Features&&... features) {
        return CreateVariant({std::forward<Features>(features)...});
    }

    std::string GenerateShaderSource(
        const std::string& baseSource,
        const ShaderVariant& variant) {
        return variant.macros + "\n" + baseSource;
    }

private:
    std::unordered_map<std::string, ShaderFeature> m_features;
    uint32_t m_featureCount = 0;

    std::string GenerateVariantId(const FeatureBits& features) {
        return std::to_string(features.to_ullong());
    }
};