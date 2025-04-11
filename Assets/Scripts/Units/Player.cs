using UnityEngine;
using UnityEngine.AI;

public class Player : Unit {
    protected override void Awake() {
        base.Awake();
        InputManager.OnPlayerMove += OnPlayerMove;
        EnableAgent();
    }

    void OnPlayerMove(Transform destinationTile) {    
        SetDestination(NavMesh.SamplePosition(destinationTile.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas) ?
            hit.position : destinationTile.position);
    }
}
