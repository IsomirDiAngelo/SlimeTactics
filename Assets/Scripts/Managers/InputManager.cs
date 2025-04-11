using System;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public static InputManager Instance { get; private set; }
    private Transform m_LastHighlightedTile;
    private Transform m_HighlightedTile;

    private Unit m_HoveredUnit;

    private Unit m_SelectedUnit;

    private bool displayGrid = false;

    // Events

    public static event Action<Transform> OnPlayerMove;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }
        Instance = this; 
    }

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        switch (GameManager.Instance.GetGameState()) {
            case GameState.Normal:
                HandleNormalState();
                break;
            case GameState.CombatPreparation:
                HandleCombatPreparationState();
                break;
            case GameState.Combat:
                HandleCombatState();
                break;
        }

        DebugEnableCombatPreparation();
        HandleSelectUnit();
    }

    void LateUpdate()
    {
        HighlightMousePosition();
    }

    // Common actions

    private void HandlePlayerMovement() {
        if (Input.GetMouseButtonDown(0)) {
            OnPlayerMove?.Invoke(m_HighlightedTile);
        }
    }

    private void HandleSelectUnit() {
        if (Input.GetMouseButtonDown(1)) {
            if (m_SelectedUnit != null) {
                m_SelectedUnit.SetSelected(false);
            }

            if (m_HoveredUnit != null) {
                m_SelectedUnit = m_HoveredUnit;
                m_SelectedUnit.SetSelected(true);
                m_SelectedUnit.DisplayStats();
            } else {
                m_SelectedUnit = null;
            }
        }
    }

    // Debug

    private void DebugEnableCombatPreparation() {
        if (Input.GetKeyDown(KeyCode.X)) {
            displayGrid = !displayGrid;
            GameManager.Instance.ChangeGameState(displayGrid ? GameState.CombatPreparation : GameState.Normal);
        }
    }

    // Normal state only

    private void HandleNormalState() {
        HandlePlayerMovement();
    }

    // Combat preparation state only

    private void HandleCombatPreparationState() {
        HandleSpawnUnit();
    }

    private void HandleSpawnUnit() {
        // Summon units on click on map
        if (Input.GetMouseButtonDown(0)) {
            UnitManager.Instance.SpawnUnit(m_HighlightedTile);
        }
    }

    // Combat state only

    private void HandleCombatState() {
        HandlePlayerMovement();
    }
    

    public Transform GetHighlightedTile() {
        return m_HighlightedTile;
    }

    // Highlight the tile hovered by the mouse cursor
    void HighlightMousePosition() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit)) {

            RenderGridOutline rgo = hit.transform.GetComponentInChildren<RenderGridOutline>();
            if (rgo != null) {
                m_HighlightedTile = hit.transform;
                
                rgo.SetHover(true);

                // Disable the previous tile highlight
                if (m_LastHighlightedTile != null && m_LastHighlightedTile.GetInstanceID() != m_HighlightedTile.GetInstanceID()) {
                    m_LastHighlightedTile.GetComponentInChildren<RenderGridOutline>().SetHover(false);
                }

                m_LastHighlightedTile = m_HighlightedTile;
            }

            if (hit.transform.TryGetComponent(out Unit unit)) {
                m_HoveredUnit = unit;
                m_HoveredUnit.SetHover(true);
            } else if (m_HoveredUnit != null) {
                m_HoveredUnit.SetHover(false);
                m_HoveredUnit = null;
            }
        }
    }
}
