using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EffectDurationController : MonoBehaviour
{
    private StatusEffectSO statusEffect;
    private StatusEffectController affectedEntity;

    private int currentFrostLanceStack;
    private int newFrostLanceStack;

    private Image statusEffectVisual;
    private TMP_Text effectDurationText;
    private Image backgroundColor;
    private float effectDuration;

    public StatusEffectSO GetStatusEffect() => statusEffect;
    public int GetCurrentFrostLanceStack() => currentFrostLanceStack;
    public void IncrementFrostLanceStack() { newFrostLanceStack++; }

    private void Awake()
    {
        statusEffectVisual = transform.GetChild(2).GetComponent<Image>();
        effectDurationText = transform.GetChild(3).GetComponent<TMP_Text>();
        backgroundColor = transform.GetChild(1).GetComponent<Image>();
    }

    public void SetEffectDurationInfo(StatusEffectSO statusEffect, StatusEffectController affectedEntity) 
    { 
        this.statusEffect = statusEffect; 
        this.affectedEntity = affectedEntity;

        effectDuration = this.statusEffect.GetDuration();
        statusEffectVisual.sprite = this.statusEffect.GetImage();
        effectDurationText.text = effectDuration.ToString("F1");

        if (statusEffect.GetIsDebuff())
            backgroundColor.color = Color.yellow;
        else
            backgroundColor.color = Color.green;

        InvokeRepeating(nameof(DurationRemaining), 0, Time.deltaTime);
    }
    
    private void DurationRemaining()
    {
        effectDuration -= Time.deltaTime;
        effectDurationText.text = effectDuration.ToString("F1");

        if (effectDuration <= 0)
        {
            affectedEntity.RemoveGOFromList(gameObject);
            CancelInvoke(nameof(DurationRemaining));
            Destroy(gameObject);
        }
    }
}
