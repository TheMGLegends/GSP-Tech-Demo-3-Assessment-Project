using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EffectDurationController : MonoBehaviour
{
    private StatusEffectSO statusEffect;
    private StatusEffectController affectedEntity;

    private AbilitiesController abilitiesController;

    private Image statusEffectVisual;
    private TMP_Text effectDurationText;
    private Image backgroundColor;
    private float effectDuration;

    private string hitOrCrit;

    private bool usedMOB;
    private int appliedTimes;
    private readonly int timesToApply = 5;

    public StatusEffectSO GetStatusEffect() => statusEffect;

    public void SetEffectDuration(float effectDuration) { this.effectDuration = effectDuration; }

    private void Awake()
    {
        statusEffectVisual = transform.GetChild(2).GetComponent<Image>();
        effectDurationText = transform.GetChild(3).GetComponent<TMP_Text>();
        backgroundColor = transform.GetChild(1).GetComponent<Image>();

        abilitiesController = FindFirstObjectByType<AbilitiesController>();
    }

    public void SetEffectDurationInfo(StatusEffectSO statusEffect, StatusEffectController affectedEntity, string hitOrCrit) 
    { 
        this.statusEffect = statusEffect; 
        this.affectedEntity = affectedEntity;
        this.hitOrCrit = hitOrCrit;

        effectDuration = this.statusEffect.GetDuration();
        statusEffectVisual.sprite = this.statusEffect.GetImage();
        effectDurationText.text = effectDuration.ToString("F1");

        if (statusEffect.GetIsDebuff())
            backgroundColor.color = Color.yellow;
        else
            backgroundColor.color = Color.green;

        if (this.statusEffect.GetStatusEffectType() == StatusEffectSO.StatusEffectTypes.FireballEffect)
            StartCoroutine(DamageOverTime(0));

        InvokeRepeating(nameof(DurationRemaining), 0, Time.deltaTime);
    }

    public void FrostLanceTargetEffect(float slowness) 
    { 
        float newMovement = affectedEntity.gameObject.GetComponent<CharacterBaseController>().GetCharacterStats().GetBaseMovementSpeed() * (1 - slowness);
        affectedEntity.GetComponent<CharacterBaseController>().SetMovementSpeed(newMovement);
    }

    private void DurationRemaining()
    {
        effectDuration -= Time.deltaTime;
        effectDurationText.text = effectDuration.ToString("F1");

        switch (statusEffect.GetStatusEffectType())
        {
            case StatusEffectSO.StatusEffectTypes.ArcaneMissileEffect:
                if (abilitiesController != null)
                {
                    if (!abilitiesController.GetFreeCast() && !usedMOB)
                    {
                        abilitiesController.SetFreeCast(true);
                        usedMOB = true;
                    }
                }
                break;
            case StatusEffectSO.StatusEffectTypes.MageArmorEffect:
                affectedEntity.GetComponent<CharacterBaseController>().SetDefenseMultiplier(0.65f);
                affectedEntity.GetComponent<PlayerController>().SetManaRegen(25);
                break;
            case StatusEffectSO.StatusEffectTypes.ToxicSpitEffect:
                affectedEntity.GetComponent<PlayerController>().SetPoisonDamage(20);
                break;
        }

        if (effectDuration <= 0)
        {
            affectedEntity.RemoveGOFromList(gameObject);

            switch (statusEffect.GetStatusEffectType())
            {
                case StatusEffectSO.StatusEffectTypes.FrostLanceEffect:
                    affectedEntity.GetComponent<CharacterBaseController>().SetMovementSpeed(affectedEntity.GetComponent<CharacterBaseController>().GetCharacterStats().GetBaseMovementSpeed());
                    break;
                case StatusEffectSO.StatusEffectTypes.FireballEffect:
                    FireballDOT();
                    break;
                case StatusEffectSO.StatusEffectTypes.MageArmorEffect:
                    affectedEntity.GetComponent<CharacterBaseController>().SetDefenseMultiplier(affectedEntity.GetComponent<CharacterBaseController>().GetCharacterStats().GetBaseDefenseMultiplier());
                    affectedEntity.GetComponent<PlayerController>().SetManaRegen(affectedEntity.GetComponent<CharacterBaseController>().GetCharacterStats().GetBaseManaRegen());
                    break;
                case StatusEffectSO.StatusEffectTypes.ToxicSpitEffect:
                    affectedEntity.GetComponent<PlayerController>().SetPoisonDamage(10);
                    break;
            }

            CancelInvoke(nameof(DurationRemaining));
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageOverTime(float delay)
    {
        while (appliedTimes < timesToApply)
        {
            FireballDOT();
            yield return new WaitForSeconds(3);
            appliedTimes++;
        }
    }

    private void FireballDOT()
    {
        int damage = 4;

        if (hitOrCrit == AttackResultStrings.hasCrit)
            damage *= 2;

        affectedEntity.GetComponent<CharacterBaseController>().GetCharacterHUDController().SetHealth(affectedEntity.GetComponent<CharacterBaseController>().GetHealth());
        affectedEntity.GetComponent<CharacterBaseController>().ReduceHealth(damage);
    }
}
