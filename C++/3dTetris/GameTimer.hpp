#pragma once
#include <Windows.h>
#include <chrono>

class GameTimer {
public:
    GameTimer() : 
        m_secondsPerCount(0.0),
        m_deltaTime(-1.0),
        m_baseTime(0),
        m_pausedTime(0),
        m_stopTime(0),
        m_previousTime(0),
        m_currentTime(0),
        m_isStopped(false) {
        __int64 countsPerSec;
        QueryPerformanceFrequency((LARGE_INTEGER*)&countsPerSec);
        m_secondsPerCount = 1.0 / (double)countsPerSec;
    }

    float TotalTime() const {
        if (m_isStopped) {
            return (float)(((m_stopTime - m_pausedTime) - m_baseTime) * m_secondsPerCount);
        }
        else {
            return (float)(((m_currentTime - m_pausedTime) - m_baseTime) * m_secondsPerCount);
        }
    }

    float DeltaTime() const { return (float)m_deltaTime; }

    void Reset() {
        __int64 currTime;
        QueryPerformanceCounter((LARGE_INTEGER*)&currTime);

        m_baseTime = currTime;
        m_previousTime = currTime;
        m_stopTime = 0;
        m_isStopped = false;
    }

    void Start() {
        if (m_isStopped) {
            __int64 startTime;
            QueryPerformanceCounter((LARGE_INTEGER*)&startTime);

            m_pausedTime += (startTime - m_stopTime);
            m_previousTime = startTime;
            m_stopTime = 0;
            m_isStopped = false;
        }
    }

    void Stop() {
        if (!m_isStopped) {
            __int64 currTime;
            QueryPerformanceCounter((LARGE_INTEGER*)&currTime);

            m_stopTime = currTime;
            m_isStopped = true;
        }
    }

    void Tick() {
        if (m_isStopped) {
            m_deltaTime = 0.0;
            return;
        }

        __int64 currTime;
        QueryPerformanceCounter((LARGE_INTEGER*)&currTime);
        m_currentTime = currTime;

        m_deltaTime = (m_currentTime - m_previousTime) * m_secondsPerCount;
        m_previousTime = m_currentTime;

        if (m_deltaTime < 0.0) {
            m_deltaTime = 0.0;
        }
    }

private:
    double m_secondsPerCount;
    double m_deltaTime;

    __int64 m_baseTime;
    __int64 m_pausedTime;
    __int64 m_stopTime;
    __int64 m_previousTime;
    __int64 m_currentTime;

    bool m_isStopped;
};