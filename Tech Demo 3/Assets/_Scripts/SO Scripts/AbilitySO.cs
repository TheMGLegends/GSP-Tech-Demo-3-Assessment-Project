using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "ScriptableObjects/Ability")]
public class AbilitySO : ScriptableObject
{
    [SerializeField, Min(0)] private int manaCost;
    [SerializeField, Min(0), Tooltip("Time until ability is cast in seconds.")] private float castingTime;
    [SerializeField, Min(0), Tooltip("Base damage of ability.")] private int basePower;
    [SerializeField, Min(0), Tooltip("Length of time until usable again in seconds.")] private float cooldown;
    [SerializeField] private StatusEffectSO statusEffect;

    public int GetManaCost() => manaCost;
    public float GetCastingTime() => castingTime;
    public int GetBasePower() => basePower;
    public float GetCooldown() => cooldown;
    public StatusEffectSO GetStatusEffect() => statusEffect;
}
