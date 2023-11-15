using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "ScriptableObjects/StatusEffect")]
public class StatusEffectSO : ScriptableObject
{
    [SerializeField, Min(0), Tooltip("How long status effect lasts in seconds.")] private float duration;
    [SerializeField, Tooltip("If false then it's a buff.")] private bool isDebuff;
    [SerializeField] private bool isStackable;
    [SerializeField, Tooltip("Which entity the status effect effects.")] private string characterTag;
    [SerializeField, Tooltip("The name of the status effect.")] private string statusEffectName;
    [SerializeField, Tooltip("The icon used to represent the status effect.")] private Sprite image;

    public float GetDuration() => duration;
    public bool GetIsDebuff() => isDebuff;
    public bool GetIsStackable() => isStackable;
    public string GetCharacterTag() => characterTag;
    public string GetStatusEffectName() => statusEffectName;
    public Sprite GetImage() => image;
}
