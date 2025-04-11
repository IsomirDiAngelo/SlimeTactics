using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitCombatStats", menuName = "ScriptableObjects/UnitCombatStats", order = 1)]
public class UnitBaseCombatStats : ScriptableObject {

    [SerializeField] protected int MaxHealth;
    protected int Health;
    [SerializeField] protected int AttackPower;
    [SerializeField] protected int Defense;
    [SerializeField] protected float AttackSpeed;

    public int GetHealth() => Health;
    public int GetAttackPower() => AttackPower;
    public int GetDefense() => Defense;
    public float GetAttackSpeed() => AttackSpeed;

    public void DisplayStats() {
        Debug.Log($"Health: {Health}, Attack: {AttackPower}, Defense: {Defense}, Attack Speed: {AttackSpeed}");
    }

    public void InitHealth() {
        Health = MaxHealth;
    }

    public void TakeDamage(int damage) {
        Health -= damage;
        if (Health <= 0) {
            Debug.Log("Unit is dead.");
        } else {
            Debug.Log($"Unit took {damage} damage. Remaining Health: {Health}");
        }
    }

    public void Heal(int healAmount) {
        Health += healAmount;
        Debug.Log($"Unit healed {healAmount} Health. Current Health: {Health}");
    }
}