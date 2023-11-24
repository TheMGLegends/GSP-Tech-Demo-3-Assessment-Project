using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "ScriptableObjects/Ability")]
public class AbilitySO : ScriptableObject
{
    public enum AbilityTypes
    {
        ArcaneMissile,
        Fireball,
        FrostLance,
        MageArmor,
        ToxicSpit
    }

    [SerializeField] private AbilityTypes abilityType;
    [SerializeField] private string abilityName;
    [SerializeField] private Sprite abilitySprite;
    [SerializeField, Min(0)] private int manaCost;
    [SerializeField, Min(0), Tooltip("Time until ability is cast in seconds.")] private float castingTime;
    [SerializeField, Min(0), Tooltip("Base damage of ability.")] private int basePower;
    [SerializeField, Min(0), Tooltip("Length of time until usable again in seconds.")] private float cooldown;
    [SerializeField] private StatusEffectSO statusEffect;
    [SerializeField] private int numberOfCasts;

    // INFO: Getter functions so variables can be accessed, but not changed
    public AbilityTypes GetAbilityType() => abilityType;
    public string GetAbilityName() => abilityName;
    public Sprite GetAbilitySprite() => abilitySprite;
    public int GetManaCost() => manaCost;
    public float GetCastingTime() => castingTime;
    public int GetBasePower() => basePower;
    public float GetCooldown() => cooldown;
    public StatusEffectSO GetStatusEffect() => statusEffect;
    public int GetNumberOfCasts() => numberOfCasts;
}
