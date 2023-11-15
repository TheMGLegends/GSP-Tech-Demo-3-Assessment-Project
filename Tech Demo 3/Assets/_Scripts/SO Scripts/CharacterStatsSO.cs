using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "ScriptableObjects/CharacterStats")]
public class CharacterStatsSO : ScriptableObject
{
    [SerializeField, Min(1)] private float baseHealth;
    [SerializeField, Min(0)] private int baseMana;
    [SerializeField, Min(0.1f)] private float baseMovementSpeed;
    [SerializeField, Min(1), Tooltip("Attack speed in seconds.")] private float baseAttackSpeed;
    [SerializeField, Min(1), Tooltip("Standard attack damage.")] private float baseDamage;
    [SerializeField, Range(0, 1), Tooltip("Lower means less damage taken.")] private float baseDefenseMultiplier;
    [SerializeField, Min(0), Tooltip("Mana regened per second.")] private int baseManaRegen;

    public int GetBaseManaRegen() => baseManaRegen;
    public float GetBaseHealth() => baseHealth;
    public float GetBaseMovementSpeed() => baseMovementSpeed;
    public int GetBaseMana() => baseMana;
    public float GetBaseAttackSpeed() => baseAttackSpeed;
    public float GetBaseDamage() => baseDamage;
    public float GetBaseDefenseMultiplier() => baseDefenseMultiplier;
}
