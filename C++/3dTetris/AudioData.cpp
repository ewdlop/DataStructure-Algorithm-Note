#include <xaudio2.h>
#include <mfapi.h>
#include <mfidl.h>
#include <mfreadwrite.h>

// XAudio2 objects
IXAudio2* g_pXAudio2 = nullptr;
IXAudio2MasteringVoice* g_pMasteringVoice = nullptr;
IXAudio2SourceVoice* g_pSourceVoice = nullptr;

struct AudioData {
    BYTE* data;
    UINT32 size;
};

bool InitAudio() {
    if (FAILED(CoInitializeEx(nullptr, COINIT_MULTITHREADED))) {
        return false;
    }

    if (FAILED(MFStartup(MF_VERSION))) {
        return false;
    }

    if (FAILED(XAudio2Create(&g_pXAudio2, 0, XAUDIO2_DEFAULT_PROCESSOR))) {
        return false;
    }

    if (FAILED(g_pXAudio2->CreateMasteringVoice(&g_pMasteringVoice))) {
        return false;
    }

    return true;
}

AudioData LoadAudioFile(const wchar_t* filename) {
    AudioData result = {nullptr, 0};
    IMFSourceReader* pReader = nullptr;
    
    if (FAILED(MFCreateSourceReaderFromURL(filename, nullptr, &pReader))) {
        return result;
    }

    // Select first audio stream
    if (FAILED(pReader->SetStreamSelection(MF_SOURCE_READER_ALL_STREAMS, FALSE)) ||
        FAILED(pReader->SetStreamSelection(MF_SOURCE_READER_FIRST_AUDIO_STREAM, TRUE))) {
        pReader->Release();
        return result;
    }

    // Set output type to PCM
    IMFMediaType* pUncompressedAudioType = nullptr;
    if (FAILED(MFCreateMediaType(&pUncompressedAudioType))) {
        pReader->Release();
        return result;
    }

    if (FAILED(pUncompressedAudioType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio)) ||
        FAILED(pUncompressedAudioType->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_PCM))) {
        pUncompressedAudioType->Release();
        pReader->Release();
        return result;
    }

    if (FAILED(pReader->SetCurrentMediaType(MF_SOURCE_READER_FIRST_AUDIO_STREAM, 
        nullptr, pUncompressedAudioType))) {
        pUncompressedAudioType->Release();
        pReader->Release();
        return result;
    }

    // Read all audio data
    std::vector<BYTE> audioData;
    IMFSample* pSample = nullptr;
    DWORD flags = 0;

    while (true) {
        if (FAILED(pReader->ReadSample(MF_SOURCE_READER_FIRST_AUDIO_STREAM, 
            0, nullptr, &flags, nullptr, &pSample))) {
            break;
        }

        if (flags & MF_SOURCE_READERF_ENDOFSTREAM) {
            break;
        }

        IMFMediaBuffer* pBuffer = nullptr;
        if (SUCCEEDED(pSample->ConvertToContiguousBuffer(&pBuffer))) {
            BYTE* pAudioData = nullptr;
            DWORD cbBuffer = 0;
            
            if (SUCCEEDED(pBuffer->Lock(&pAudioData, nullptr, &cbBuffer))) {
                audioData.insert(audioData.end(), pAudioData, pAudioData + cbBuffer);
                pBuffer->Unlock();
            }
            pBuffer->Release();
        }
        pSample->Release();
    }

    if (!audioData.empty()) {
        result.size = (UINT32)audioData.size();
        result.data = new BYTE[result.size];
        memcpy(result.data, audioData.data(), result.size);
    }

    pUncompressedAudioType->Release();
    pReader->Release();
    return result;
}

void PlaySound(const AudioData& audio) {
    if (!audio.data || !audio.size) return;

    WAVEFORMATEX waveFormat = {};
    waveFormat.wFormatTag = WAVE_FORMAT_PCM;
    waveFormat.nChannels = 2;
    waveFormat.nSamplesPerSec = 44100;
    waveFormat.wBitsPerSample = 16;
    waveFormat.nBlockAlign = (waveFormat.nChannels * waveFormat.wBitsPerSample) / 8;
    waveFormat.nAvgBytesPerSec = waveFormat.nSamplesPerSec * waveFormat.nBlockAlign;

    if (SUCCEEDED(g_pXAudio2->CreateSourceVoice(&g_pSourceVoice, &waveFormat))) {
        XAUDIO2_BUFFER buffer = {};
        buffer.pAudioData = audio.data;
        buffer.AudioBytes = audio.size;
        buffer.Flags = XAUDIO2_END_OF_STREAM;
        buffer.LoopCount = XAUDIO2_LOOP_INFINITE;  // For background music

        g_pSourceVoice->SubmitSourceBuffer(&buffer);
        g_pSourceVoice->Start();
    }
}

void CleanupAudio() {
    if (g_pSourceVoice) {
        g_pSourceVoice->Stop();
        g_pSourceVoice->DestroyVoice();
    }
    if (g_pMasteringVoice) {
        g_pMasteringVoice->DestroyVoice();
    }
    if (g_pXAudio2) {
        g_pXAudio2->Release();
    }
    MFShutdown();
    CoUninitialize();
}