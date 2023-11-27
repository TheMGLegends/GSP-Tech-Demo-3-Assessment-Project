using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the duration of the status effect (Basically the length of time that the effects should be applied for)
/// </summary>
public class EffectDurationController : MonoBehaviour
{
    private StatusEffectSO statusEffect;
    private StatusEffectController affectedEntity;
    private CharacterBaseController baseController;

    private AbilitiesController abilitiesController;

    private Image statusEffectVisual;
    private TMP_Text effectDurationText;
    private Image backgroundColor;
    private TMP_Text stackText;
    private float effectDuration;

    private string hitOrCrit;

    private bool usedMOB;
    private int appliedTimes;
    private readonly int timesToApply = 5;

    public StatusEffectSO GetStatusEffect() => statusEffect;

    public void SetEffectDuration(float effectDuration) { this.effectDuration = effectDuration; }
    public void SetStackText(int stack) { stackText.text = stack.ToString(); }

    private void Awake()
    {
        statusEffectVisual = transform.GetChild(2).GetComponent<Image>();
        effectDurationText = transform.GetChild(3).GetComponent<TMP_Text>();
        backgroundColor = transform.GetChild(1).GetComponent<Image>();
        stackText = transform.GetChild(4).GetComponent<TMP_Text>();

        abilitiesController = FindFirstObjectByType<AbilitiesController>();
    }

    public void SetEffectDurationInfo(StatusEffectSO statusEffect, StatusEffectController affectedEntity, string hitOrCrit) 
    { 
        this.statusEffect = statusEffect; 
        this.affectedEntity = affectedEntity;
        this.hitOrCrit = hitOrCrit;

        baseController = this.affectedEntity.GetComponent<CharacterBaseController>();

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
        float newMovement = baseController.GetCharacterStats().GetBaseMovementSpeed() * (1 - slowness);
        baseController.SetMovementSpeed(newMovement);
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
                baseController.SetDefenseMultiplier(0.65f);
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
                    baseController.SetMovementSpeed(baseController.GetCharacterStats().GetBaseMovementSpeed());
                    affectedEntity.SetCurrentFrostLanceStack(0);
                    affectedEntity.SetSlownessPercentage(0);
                    break;
                case StatusEffectSO.StatusEffectTypes.FireballEffect:
                    FireballDOT();
                    break;
                case StatusEffectSO.StatusEffectTypes.MageArmorEffect:
                    baseController.SetDefenseMultiplier(baseController.GetCharacterStats().GetBaseDefenseMultiplier());
                    affectedEntity.GetComponent<PlayerController>().SetManaRegen(baseController.GetCharacterStats().GetBaseManaRegen());
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

        baseController.ReduceHealth(damage);
        baseController.GetCharacterHUDController().SetHealth(baseController.GetHealth());
    }
}
