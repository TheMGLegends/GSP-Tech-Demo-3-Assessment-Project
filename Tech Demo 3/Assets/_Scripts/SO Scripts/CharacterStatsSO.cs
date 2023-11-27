using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "ScriptableObjects/CharacterStats")]
public class CharacterStatsSO : ScriptableObject
{
    public enum CharacterTypes
    {
        Player,
        Enemy
    }

    [SerializeField] private CharacterTypes characterType;
    [SerializeField, Min(1)] private float baseHealth;
    [SerializeField, Min(0)] private int baseMana;
    [SerializeField, Min(0.1f)] private float baseMovementSpeed;
    [SerializeField, Range(0, 1), Tooltip("Lower means less damage taken.")] private float baseDefenseMultiplier;
    [SerializeField, Min(0), Tooltip("Mana regened per second.")] private int baseManaRegen;
    [SerializeField, Min(1), Tooltip("Standard attack damage.")] private float normalDamageAmount;
    [SerializeField, Min(1), Tooltip("Attack speed in seconds.")] private float normalAttackSpeed;

    // INFO: Getter functions so variables can be accessed, but not changed
    public CharacterTypes GetCharacterType() => characterType;
    public float GetBaseHealth() => baseHealth;
    public int GetBaseMana() => baseMana;
    public float GetBaseMovementSpeed() => baseMovementSpeed;
    public float GetBaseDefenseMultiplier() => baseDefenseMultiplier;
    public int GetBaseManaRegen() => baseManaRegen;
    public float GetNormalDamage() => normalDamageAmount;
    public float GetNormalAttackSpeed() => normalAttackSpeed;
}
