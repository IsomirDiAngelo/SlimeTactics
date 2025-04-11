using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action<GameState> OnGameStateChanged;
    private GameState m_GameState;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    
    void Start() {
        m_GameState = GameState.Normal;
    }

    public GameState GetGameState() {
        return m_GameState;
    }

    public void ChangeGameState(GameState state) {
        m_GameState = state;

        switch (m_GameState) {
            case GameState.Normal:
                break;
            case GameState.CombatPreparation:
                HandleCombatPreparation();
                break;
            case GameState.Combat:
                break;
        }

        OnGameStateChanged?.Invoke(m_GameState);
    }

    private void HandleCombatPreparation() {

    }

    private void HandleCombat() {

    }

}

public enum GameState {
    Normal,
    CombatPreparation,
    Combat
}
