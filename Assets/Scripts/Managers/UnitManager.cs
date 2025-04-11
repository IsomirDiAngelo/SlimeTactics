using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour {
    public static UnitManager Instance { get; private set; }

    private List<Unit> m_Units;

    [SerializeField] private Player m_Player;
    [SerializeField] private DummyEnemy m_DummyEnemy;
    [SerializeField] private DummyAlly m_DummyAlly;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        m_Units = new List<Unit> { 
            m_Player
        };
    }

    public List<Unit> GetUnits() {
        return m_Units;
    }

    public void AddUnit(Unit unit) {
        m_Units.Add(unit);
    }

    public void RemoveUnit(Unit unit) {
        m_Units.Remove(unit);
    }

    public Player GetPlayer() {
        return m_Player;
    }

    public void SpawnUnit(Transform tile) {
        if (NavMesh.SamplePosition(tile.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas)) {
            DummyAlly newAlly = Instantiate(m_DummyAlly, hit.position, Quaternion.identity);
            newAlly.EnableAgent();
            AddUnit(newAlly);
        }
    }
}
