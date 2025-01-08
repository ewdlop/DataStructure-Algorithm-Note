#include "Audio.h"

Audio::Audio() : m_xAudio2(nullptr), m_masteringVoice(nullptr) {
    // Initialize array of source voices
    for (int i = 0; i < SOUND_TYPE_COUNT; i++) {
        m_sourceVoices[i] = nullptr;
    }
}

Audio::~Audio() {
    Cleanup();
}

bool Audio::Initialize() {
    HRESULT hr;

    // Initialize COM and MF
    hr = CoInitializeEx(nullptr, COINIT_MULTITHREADED);
    if (FAILED(hr)) return false;

    hr = MFStartup