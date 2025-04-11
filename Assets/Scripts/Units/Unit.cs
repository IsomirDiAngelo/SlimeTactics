using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {

    // Components
    protected Rigidbody m_Rigidbody;
    protected NavMeshAgent m_NavMeshAgent;

    // Behavior
    protected UnitBehaviorState m_State { get; set; }
    protected Unit m_Target;
    protected Dictionary<Unit, float> m_DamageDealers; // TODO: Clear dictionary of dead units
    protected float m_AttackTimer;

    // Stats

    [SerializeField] protected UnitType m_UnitType;
    [SerializeField] protected UnitBaseCombatStats m_BaseCombatStats;
    [SerializeField] protected float m_AttackRange = .32f;
    [SerializeField] protected float m_TargetRange = .96f;

    // Animations 

    protected UnitAnimationState m_AnimationState { get; set; }

    protected Animator m_Animator;

    protected Outline m_Outline;

    protected bool isSelected = false;
    protected bool isHovered = false;

    // Movement

    private Quaternion m_TargetRotation;
    [SerializeField] protected float m_TurnSpeed = 10f;

    protected virtual void Awake() {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();

        m_Outline = GetComponent<Outline>();
        m_Outline.enabled = false;

        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_NavMeshAgent.enabled = false;
        m_NavMeshAgent.updateRotation = false;

        m_AttackTimer = 0f;

        m_BaseCombatStats = Instantiate(m_BaseCombatStats);
        m_BaseCombatStats.InitHealth();

        if (m_Animator) {
            m_Animator.SetFloat("AttackSpeed", m_BaseCombatStats.GetAttackSpeed());
        }

        m_DamageDealers = new Dictionary<Unit, float>();
    }

    protected virtual void FixedUpdate() {
        // if (Quaternion.Angle(transform.rotation, m_TargetRotation) > .1f) {
        //     TurnUnit();
        // }
    }

    void Update() {
        if (Quaternion.Angle(transform.rotation, m_TargetRotation) > .1f) {
            TurnUnit();
        }

        if (m_Animator) {
            m_Animator.SetBool("IsWalking", m_NavMeshAgent.velocity.magnitude > 0 || m_State == UnitBehaviorState.ChasingTarget);
            m_Animator.SetBool("IsAttacking", m_State == UnitBehaviorState.AutoAttacking && m_AttackTimer > 0f);
            m_Animator.SetFloat("AttackSpeed", m_BaseCombatStats.GetAttackSpeed());
        }

        UpdateOutlineRendering();
    }

    private void UpdateOutlineRendering() {
        m_Outline.enabled = isHovered || isSelected;
        m_Outline.OutlineColor = isSelected ? Color.yellow : isHovered ? Color.white : Color.white;
    }

    public void SetDestination(Vector3 destination) {
        RotateTowards(destination);
        m_NavMeshAgent.SetDestination(destination);
    }

    protected void RotateTowards(Vector3 destination) {
        Vector3 direction = (destination - transform.position).normalized;
        direction.y = 0;
        if (direction.magnitude > 0) m_TargetRotation = Quaternion.LookRotation(direction);
    }

    private void TurnUnit() {
        transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRotation, Time.deltaTime * m_TurnSpeed);
    }

    public Rigidbody GetRigidbody() {
        return m_Rigidbody;
    }

    public void EnableAgent() {
        m_NavMeshAgent.enabled = true;
    }

    public void DisableAgent() {
        m_NavMeshAgent.enabled = false;
    }

    public void SetOutlineStatus(bool enable) {
        m_Outline.enabled = enable;
    }

    public void SetOutlineColor(Color color) {
        m_Outline.OutlineColor = color;
    }

    public void Select() {
        SetOutlineColor(Color.yellow);
    }

    public void SetHover(bool hover) {
        isHovered = hover;
    }

    public void SetSelected(bool selected) {
        isSelected = selected;
    }

    // Follow target from a set distance
    protected void FollowTarget(Rigidbody targetRigidbody, float followDistance) {
        Vector3 distanceVector = targetRigidbody.position - m_Rigidbody.position;
        Vector3 direction = distanceVector.normalized;
        float distance = distanceVector.magnitude;
        if (Math.Abs(distance - followDistance) <= .05f) {
            SetDestination(m_Rigidbody.position);
        } else {
            SetDestination(targetRigidbody.position - direction * followDistance);
        }
    }

    // Pick the nearest target in a targetRange radius tagged as unitType
    protected Unit PickNearestTarget(UnitType unitType) {
        Collider[] colliders = Physics.OverlapSphere(m_Rigidbody.position, m_TargetRange);
        Unit target = null;
        float closestDistance = float.MaxValue;

        foreach (Collider collider in colliders) {
            if (collider.TryGetComponent(out Unit unit) && unit.m_UnitType == unitType) {
                float distance = Vector3.Distance(m_Rigidbody.position, unit.GetRigidbody().position);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    target = unit;
                }
            }
        }

        return target;
    }

    // Pick the target with the most damage in a targetRange radius
    protected Unit PickMostDamageDealer(UnitType unitType) {
        if (m_DamageDealers.Count == 0) {
            return null;
        }

        Collider[] colliders = Physics.OverlapSphere(m_Rigidbody.position, m_TargetRange);
        Unit target = null;
        float mostDamage = float.MinValue;

        foreach (Collider collider in colliders) {
            if (collider.TryGetComponent(out Unit unit) && unit.m_UnitType == unitType) {
                if (m_DamageDealers.ContainsKey(unit)) {
                    float damage = m_DamageDealers[unit];
                    if (damage > mostDamage) {
                        mostDamage = damage;
                        target = unit;
                    }
                }
            }
        }

        return target;
    }

    protected bool IsInAttackRange(Unit target) {
        return Math.Abs(Vector3.Distance(m_Rigidbody.position, target.GetRigidbody().position) - m_AttackRange) <= .05f;
    }
    
    protected bool IsTargetLost() {
        return m_Target == null || (m_Target != null && m_AttackTimer == 0f && !IsInAttackRange(m_Target));
    }
    
    public void TakeDamage(int damage, Unit damageDealer) {
        m_BaseCombatStats.TakeDamage(damage); // Temporary damage calculation
        m_DamageDealers[damageDealer] = m_DamageDealers.GetValueOrDefault(damageDealer, 0) + damage;

        if (m_BaseCombatStats.GetHealth() <= 0) {
            Destroy(gameObject);
        }
    }

    public void Heal(int healAmount) {
        m_BaseCombatStats.Heal(healAmount);
    }

    public void AutoAttack() {
        m_AttackTimer += Time.deltaTime;
        if (m_AttackTimer >= 1 / m_BaseCombatStats.GetAttackSpeed()) {
            m_AttackTimer = 0f; 
            m_Target.TakeDamage(m_BaseCombatStats.GetAttackPower(), this);
        }
    }

    public void DisplayStats() {
        m_BaseCombatStats.DisplayStats();
    }
}


public enum UnitType {
    Ally,
    Enemy,
    Neutral
}

public enum UnitBehaviorState {
    Idle, // Static
    FollowingPlayer, // No dangerous enemies in sight
    Patroling,
    ChasingTarget, // Dangerous enemies in sight
    AutoAttacking, 

    Casting,
    Dying,
}

public enum UnitAnimationState {
    Idle,
    Walking,
    Attacking,
    Casting,
    TakingDamage,
    Dying,
    Dead
}