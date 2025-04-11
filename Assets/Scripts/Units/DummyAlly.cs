using UnityEngine;

public class DummyAlly : Unit {

    [SerializeField] private float m_followDistance = .64f;
    
    void Start() {
        m_State = UnitBehaviorState.FollowingPlayer;
    }

    // protected override void Update() {
    //     base.Update();
    // void Update() {
    protected override void FixedUpdate() {
        base.FixedUpdate();
        switch (m_State) {
            case UnitBehaviorState.Idle:
                return;
            case UnitBehaviorState.FollowingPlayer:
                FollowPlayer();
                SearchNearestTarget();
                break;
            case UnitBehaviorState.ChasingTarget:
                if (m_Target != null) {
                    FollowTarget(m_Target.GetRigidbody(), m_AttackRange);

                    if (IsInAttackRange(m_Target)) {
                        m_AttackTimer = 0f;
                        m_State = UnitBehaviorState.AutoAttacking;
                        RotateTowards(m_Target.GetRigidbody().position);
                    }
                } else {
                    m_State = UnitBehaviorState.FollowingPlayer;
                }
                break;
            case UnitBehaviorState.AutoAttacking:
                AutoAttack();
                if (IsTargetLost()) {
                    m_State = UnitBehaviorState.FollowingPlayer;
                } 
                break;
        }
    }

    private void FollowPlayer() {
        FollowTarget(UnitManager.Instance.GetPlayer().GetRigidbody(), m_followDistance);
    }

    private void SearchNearestTarget() {
        m_Target = PickNearestTarget(UnitType.Enemy);
        if (m_Target != null) {
            m_State = UnitBehaviorState.ChasingTarget;
        }
    }
}