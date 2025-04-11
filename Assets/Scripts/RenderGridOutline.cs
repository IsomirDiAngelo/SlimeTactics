using UnityEngine;

public class RenderGridOutline : MonoBehaviour {
    private MeshRenderer m_Renderer;
    private bool m_Hover = false;
    private bool m_CombatPreparation = false;

    void Awake() {
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    void Start() {
        m_Renderer = transform.GetComponent<MeshRenderer>();
        UpdateRendering();
    }

    private void OnGameStateChanged(GameState gameState) {
        m_CombatPreparation = gameState == GameState.CombatPreparation;
        UpdateRendering();
    }

    public void SetHover(bool hover) {
        m_Hover = hover;
        UpdateRendering();
    }

    private void UpdateRendering() {
        m_Renderer.material.SetFloat("_Size", m_Hover ? 0f : 0.8f);
        m_Renderer.material.SetFloat("_Alpha", m_Hover ? 0.4f : m_CombatPreparation ? 0.2f : 0.0f);
    }
}
