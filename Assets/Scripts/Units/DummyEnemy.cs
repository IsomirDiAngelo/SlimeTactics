using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DummyEnemy : Unit {
    void Start() {
        m_NavMeshAgent.Warp(NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas) ?
            hit.position : transform.position);
        EnableAgent();

        m_State = UnitBehaviorState.Idle;
    }

    // protected override void Update() {
    //     base.Update();
    protected override void FixedUpdate() {
        base.FixedUpdate();
        switch (m_State) {
            case UnitBehaviorState.Idle:
                SearchNearestTarget();
                return;
            case UnitBehaviorState.ChasingTarget:
                Unit targetSwitch = PickMostDamageDealer(UnitType.Ally); // Switch target if the most damaging unit in range is not the current target
                if (targetSwitch) {
                    m_Target = targetSwitch;
                }

                if (m_Target != null) {
                    FollowTarget(m_Target.GetRigidbody(), m_AttackRange);

                    if (IsInAttackRange(m_Target)) {
                        m_AttackTimer = 0f;
                        m_State = UnitBehaviorState.AutoAttacking;
                        RotateTowards(m_Target.GetRigidbody().position);
                    }
                } else {
                    m_State = UnitBehaviorState.Idle;
                }
                break;
            case UnitBehaviorState.AutoAttacking:
                AutoAttack();
                if (IsTargetLost()) {
                    m_State = UnitBehaviorState.ChasingTarget;
                }
                break;
        }
    }

    private void SearchNearestTarget() {
        m_Target = PickNearestTarget(UnitType.Ally);
        if (m_Target != null) {
            m_State = UnitBehaviorState.ChasingTarget;
        }
    }

    // private void OnTriggerEnter(Collider other) {
    //     Debug.Log("Hello");
    //     if (m_State == UnitBehaviorState.Idle && other.TryGetComponent(out Unit unit)) {
    //         m_Target = unit;
    //         m_State = UnitBehaviorState.ChasingTarget;
    //     }
    // }
}