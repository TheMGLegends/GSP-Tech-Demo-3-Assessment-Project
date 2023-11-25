using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDurationController : MonoBehaviour
{
    private StatusEffectSO statusEffect;
    private int currentFrostLanceStack;
    private int newFrostLanceStack;

    public StatusEffectSO GetStatusEffect() => statusEffect;
    public int GetCurrentFrostLanceStack() => currentFrostLanceStack;

    public void IncrementFrostLanceStack() { newFrostLanceStack++; }

    public void SetStatusEffect(StatusEffectSO statusEffect) { this.statusEffect = statusEffect; } 
}
