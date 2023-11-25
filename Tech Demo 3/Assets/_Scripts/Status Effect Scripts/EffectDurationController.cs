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
    private float slownessPercentage;

    private Image statusEffectVisual;
    private TMP_Text effectDurationText;
    private Image backgroundColor;
    private float effectDuration;

    public StatusEffectSO GetStatusEffect() => statusEffect;
    public int GetCurrentFrostLanceStack() => currentFrostLanceStack;

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

    public void IncrementFrostLanceStack() 
    { 
        currentFrostLanceStack++;

        if (statusEffect.GetStatusEffectType() == StatusEffectSO.StatusEffectTypes.FrostLanceEffect)
        {
            effectDuration = statusEffect.GetDuration();
            slownessPercentage += 0.15f;

            float newMovement = affectedEntity.gameObject.GetComponent<CharacterBaseController>().GetCharacterStats().GetBaseMovementSpeed() * (1 - slownessPercentage);
            affectedEntity.GetComponent<CharacterBaseController>().SetMovementSpeed(newMovement);
        }
    }

    private void DurationRemaining()
    {
        effectDuration -= Time.deltaTime;
        effectDurationText.text = effectDuration.ToString("F1");

        if (effectDuration <= 0)
        {
            affectedEntity.RemoveGOFromList(gameObject);

            // LEFT OFF HERE:
            switch (statusEffect.GetStatusEffectType())
            {
                case StatusEffectSO.StatusEffectTypes.ArcaneMissileEffect:
                    break;
                case StatusEffectSO.StatusEffectTypes.FireballEffect:
                    break;
                case StatusEffectSO.StatusEffectTypes.FrostLanceEffect:
                    break;
                case StatusEffectSO.StatusEffectTypes.MageArmorEffect:
                    break;
                case StatusEffectSO.StatusEffectTypes.ToxicSpitEffect:
                    break;
                default:
                    break;
            }

            if (statusEffect.GetStatusEffectType() == StatusEffectSO.StatusEffectTypes.FrostLanceEffect)
                affectedEntity.GetComponent<CharacterBaseController>().SetMovementSpeed(affectedEntity.GetComponent<CharacterBaseController>().GetCharacterStats().GetBaseMovementSpeed());

            CancelInvoke(nameof(DurationRemaining));
            Destroy(gameObject);
        }
    }
}
