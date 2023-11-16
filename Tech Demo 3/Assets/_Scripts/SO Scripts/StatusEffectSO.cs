using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "ScriptableObjects/StatusEffect")]
public class StatusEffectSO : ScriptableObject
{
    public enum StatusEffectTypes
    {
        ArcaneMissileEffect,
        FireballEffect,
        FrostLanceEffect,
        MageArmorEffect,
        ToxicSpitEffect
    }

    [SerializeField] private StatusEffectTypes statusEffectType;
    [SerializeField, Tooltip("The icon used to represent the status effect.")] private Sprite image;
    [SerializeField, Min(0), Tooltip("How long status effect lasts in seconds.")] private float duration;
    [SerializeField, Tooltip("If false then it's a buff.")] private bool isDebuff;
    [SerializeField] private bool isStackable;

    public StatusEffectTypes GetStatusEffectType() => statusEffectType;
    public Sprite GetImage() => image;
    public float GetDuration() => duration;
    public bool GetIsDebuff() => isDebuff;
    public bool GetIsStackable() => isStackable;
}
