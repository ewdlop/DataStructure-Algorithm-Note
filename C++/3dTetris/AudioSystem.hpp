#pragma once
#include <xaudio2.h>
#include <mfapi.h>
#include <mfidl.h>
#include <mfreadwrite.h>
#include <string>
#include <vector>

class AudioSystem {
public:
    enum SoundEffect {
        MOVE,
        ROTATE,
        DROP,
        LINE_CLEAR,
        LEVEL_UP,
        GAME_OVER,
        BACKGROUND_MUSIC,
        SOUND_COUNT
    };

    AudioSystem();
    ~AudioSystem();

    bool Initialize();
    void PlaySound(SoundEffect effect, bool loop = false);
    void StopSound(SoundEffect effect);
    void SetVolume(SoundEffect effect, float volume);
    void StopAll();
    void PauseAll();
    void ResumeAll();
    void Cleanup();

private:
    IXAudio2* m_pXAudio2;
    IXAudio2MasteringVoice* m_pMasteringVoice;
    
    struct AudioBuffer {
        std::vector<BYTE> audioData;
        WAVEFORMATEX waveFormat;
        IXAudio2SourceVoice* sourceVoice;
        bool isLoaded;
    };

    AudioBuffer m_audioBuffers[SOUND_COUNT];
    const wchar_t* m_soundFiles[SOUND_COUNT] = {
        L"move.wav",
        L"rotate.wav",
        L"drop.wav",
        L"line_clear.wav",
        L"level_up.wav",
        L"game_over.wav",
        L"background.wav"
    };

    bool LoadSound(const wchar_t* filename, AudioBuffer& buffer);
    bool CreateSourceVoice(AudioBuffer& buffer);
};

// Implementation
AudioSystem::AudioSystem() : m_pXAudio2(nullptr), m_pMasteringVoice(nullptr) {
    for (auto& buffer : m_audioBuffers) {
        buffer.sourceVoice = nullptr;
        buffer.isLoaded = false;
    }
}

bool AudioSystem::Initialize() {
    HRESULT hr;

    // Initialize COM and MF
    hr = CoInitializeEx(nullptr, COINIT_MULTITHREADED);
    if (FAILED(hr)) return false;

    hr = MFStartup(MF_VERSION);
    if (FAILED(hr)) return false;

    // Create XAudio2 instance
    hr = XAudio2Create(&m_pXAudio2);
    if (FAILED(hr)) return false;

    // Create mastering voice
    hr = m_pXAudio2->CreateMasteringVoice(&m_pMasteringVoice);
    if (FAILED(hr)) return false;

    // Load all sound effects
    for (int i = 0; i < SOUND_COUNT; i++) {
        if (!LoadSound(m_soundFiles[i], m_audioBuffers[i])) {
            return false;
        }
    }

    return true;
}

bool AudioSystem::LoadSound(const wchar_t* filename, AudioBuffer& buffer) {
    IMFSourceReader* pReader = nullptr;
    HRESULT hr = MFCreateSourceReaderFromURL(filename, nullptr, &pReader);
    if (FAILED(hr)) return false;

    // Configure source reader
    hr = pReader->SetStreamSelection(MF_SOURCE_READER_ALL_STREAMS, FALSE);
    hr = pReader->SetStreamSelection(MF_SOURCE_READER_FIRST_AUDIO_STREAM, TRUE);

    // Get native format
    IMFMediaType* pNativeType = nullptr;
    hr = pReader->GetNativeMediaType(MF_SOURCE_READER_FIRST_AUDIO_STREAM, 0, &pNativeType);
    if (FAILED(hr)) {
        pReader->Release();
        return false;
    }

    // Convert to PCM format
    IMFMediaType* pPCMType = nullptr;
    hr = MFCreateMediaType(&pPCMType);
    hr = pPCMType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
    hr = pPCMType->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_PCM);
    hr = pReader->SetCurrentMediaType(MF_SOURCE_READER_FIRST_AUDIO_STREAM, nullptr, pPCMType);

    // Read audio data
    buffer.audioData.clear();
    while (true) {
        IMFSample* pSample = nullptr;
        DWORD flags = 0;
        hr = pReader->ReadSample(MF_SOURCE_READER_FIRST_AUDIO_STREAM, 0, nullptr, &flags, nullptr, &pSample);
        
        if (flags & MF_SOURCE_READERF_ENDOFSTREAM) break;

        if (pSample) {
            IMFMediaBuffer* pBuffer = nullptr;
            hr = pSample->ConvertToContiguousBuffer(&pBuffer);
            
            BYTE* audioData = nullptr;
            DWORD audioDataLength = 0;
            hr = pBuffer->Lock(&audioData, nullptr, &audioDataLength);
            
            buffer.audioData.insert(buffer.audioData.end(), audioData, audioData + audioDataLength);
            
            pBuffer->Unlock();
            pBuffer->Release();
            pSample->Release();
        }
    }

    // Create source voice
    return CreateSourceVoice(buffer);
}

bool AudioSystem::CreateSourceVoice(AudioBuffer& buffer) {
    XAUDIO2_BUFFER xBuffer = {};
    xBuffer.AudioBytes = buffer.audioData.size();
    xBuffer.pAudioData = buffer.audioData.data();
    xBuffer.Flags = XAUDIO2_END_OF_STREAM;

    HRESULT hr = m_pXAudio2->CreateSourceVoice(&buffer.sourceVoice, &buffer.waveFormat);
    if (FAILED(hr)) return false;

    hr = buffer.sourceVoice->SubmitSourceBuffer(&xBuffer);
    if (FAILED(hr)) return false;

    buffer.isLoaded = true;
    return true;
}

void AudioSystem::PlaySound(SoundEffect effect, bool loop) {
    if (effect >= 0 && effect < SOUND_COUNT && m_audioBuffers[effect].isLoaded) {
        m_audioBuffers[effect].sourceVoice->Stop();
        m_audioBuffers[effect].sourceVoice->FlushSourceBuffers();
        
        XAUDIO2_BUFFER buffer = {};
        buffer.AudioBytes = m_audioBuffers[effect].audioData.size();
        buffer.pAudioData = m_audioBuffers[effect].audioData.data();
        buffer.Flags = XAUDIO2_END_OF_STREAM;
        buffer.LoopCount = loop ? XAUDIO2_LOOP_INFINITE : 0;

        m_audioBuffers[effect].sourceVoice->SubmitSourceBuffer(&buffer);
        m_audioBuffers[effect].sourceVoice->Start();
    }
}

void AudioSystem::Cleanup() {
    for (auto& buffer : m_audioBuffers) {
        if (buffer.sourceVoice) {
            buffer.sourceVoice->Stop();
            buffer.sourceVoice->DestroyVoice();
            buffer.sourceVoice = nullptr;
        }
    }

    if (m_pMasteringVoice) {
        m_pMasteringVoice->DestroyVoice();
        m_pMasteringVoice = nullptr;
    }

    if (m_pXAudio2) {
        m_pXAudio2->Release();
        m_pXAudio2 = nullptr;
    }

    MFShutdown();
    CoUninitialize();
}