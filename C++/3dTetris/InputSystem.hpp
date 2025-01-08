#pragma once
#include <Windows.h>
#include <queue>
#include <array>
#include <unordered_map>
#include <optional>

class InputSystem {
public:
    enum class Action {
        MOVE_LEFT,
        MOVE_RIGHT,
        MOVE_FORWARD,
        MOVE_BACKWARD,
        ROTATE_X,
        ROTATE_Y,
        ROTATE_Z,
        HARD_DROP,
        SOFT_DROP,
        HOLD_PIECE,
        PAUSE,
        NONE
    };

    struct InputConfig {
        static constexpr float DEFAULT_REPEAT_DELAY = 0.2f;
        static constexpr float DEFAULT_REPEAT_RATE = 0.05f;
        static constexpr float SLOW_REPEAT_RATE = 0.2f;
    };

    struct KeyState {
        bool isPressed = false;
        float timeHeld = 0.0f;
        float repeatDelay = InputConfig::DEFAULT_REPEAT_DELAY;
        float repeatRate = InputConfig::DEFAULT_REPEAT_RATE;
    };

private:
    static constexpr std::array<std::pair<WPARAM, Action>, 10> KEY_MAPPINGS = {{
        {VK_LEFT, Action::MOVE_LEFT},
        {VK_RIGHT, Action::MOVE_RIGHT},
        {VK_UP, Action::MOVE_FORWARD},
        {VK_DOWN, Action::MOVE_BACKWARD},
        {'X', Action::ROTATE_X},
        {'Y', Action::ROTATE_Y},
        {'Z', Action::ROTATE_Z},
        {VK_SPACE, Action::HARD_DROP},
        {'C', Action::HOLD_PIECE},
        {VK_ESCAPE, Action::PAUSE}
    }};

    std::queue<Action> m_inputQueue;
    std::unordered_map<WPARAM, KeyState> m_keyStates;

public:
    InputSystem() {
        // Initialize key states with configured repeat rates
        for (const auto& [key, action] : KEY_MAPPINGS) {
            KeyState state;
            // Set slower repeat rate for rotations
            if (action == Action::ROTATE_X || 
                action == Action::ROTATE_Y || 
                action == Action::ROTATE_Z) {
                state.repeatRate = InputConfig::SLOW_REPEAT_RATE;
            }
            m_keyStates[key] = state;
        }
    }

    void Update(float deltaTime) {
        for (auto& [key, state] : m_keyStates) {
            if (state.isPressed) {
                state.timeHeld += deltaTime;
                if (state.timeHeld > state.repeatDelay) {
                    float repeatTime = state.timeHeld - state.repeatDelay;
                    if (repeatTime >= state.repeatRate) {
                        if (auto action = KeyToAction(key)) {
                            m_inputQueue.push(*action);
                        }
                        state.timeHeld = state.repeatDelay + 
                            std::fmod(repeatTime, state.repeatRate);
                    }
                }
            }
        }
    }

    void KeyDown(WPARAM key) {
        if (auto it = m_keyStates.find(key); it != m_keyStates.end()) {
            if (!it->second.isPressed) {
                it->second.isPressed = true;
                it->second.timeHeld = 0.0f;
                if (auto action = KeyToAction(key)) {
                    m_inputQueue.push(*action);
                }
            }
        }
    }

    void KeyUp(WPARAM key) {
        if (auto it = m_keyStates.find(key); it != m_keyStates.end()) {
            it->second.isPressed = false;
            it->second.timeHeld = 0.0f;
        }
    }

    std::optional<Action> GetNextAction() {
        if (m_inputQueue.empty()) return std::nullopt;
        Action action = m_inputQueue.front();
        m_inputQueue.pop();
        return action;
    }

private:
    std::optional<Action> KeyToAction(WPARAM key) const {
        for (const auto& [mappedKey, action] : KEY_MAPPINGS) {
            if (mappedKey == key) return action;
        }
        return std::nullopt;
    }
};