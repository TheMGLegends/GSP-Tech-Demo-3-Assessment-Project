using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the status effects that are present on the character
/// </summary>
public class StatusEffectController : MonoBehaviour
{
    [SerializeField] private GameObject charactersEffectsPanel;
    private List<GameObject> effectsList = new();
    private GameObject statusEffectPrefab;

    private AbilitySO usedSpellInfo;

    private readonly int maxFrostLanceStack = 5;
    private int currentFrostLanceStack;
    private float slownessPercentage;

    private string hitOrCrit;

    public int GetCurrentFrostLanceStack() => currentFrostLanceStack;
    public void SetSlownessPercentage(float slownessPercentage) { this.slownessPercentage =  slownessPercentage; }
    public void SetCurrentFrostLanceStack(int currentFrostLanceStack) { this.currentFrostLanceStack = currentFrostLanceStack; }

    private void Start()
    {
        statusEffectPrefab = ReferenceManager.Instance.statusEffectPrefab;

        foreach (Transform child in charactersEffectsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CheckActiveEffects()
    {
        for (int i = 0; i < effectsList.Count; i++)
        {
            EffectDurationController durationController = effectsList[i].GetComponent<EffectDurationController>();

            if (currentFrostLanceStack == 0 && usedSpellInfo.GetStatusEffect().GetStatusEffectType() == StatusEffectSO.StatusEffectTypes.FrostLanceEffect)
                currentFrostLanceStack++;

            if (usedSpellInfo.GetStatusEffect() == durationController.GetStatusEffect())
            {
                if (durationController.GetStatusEffect().GetStatusEffectType() == StatusEffectSO.StatusEffectTypes.FrostLanceEffect)
                {
                    if (currentFrostLanceStack < maxFrostLanceStack)
                    {
                        currentFrostLanceStack++;
                        durationController.SetStackText(currentFrostLanceStack);
                        slownessPercentage = 0.15f * currentFrostLanceStack;
                        durationController.SetEffectDuration(usedSpellInfo.GetStatusEffect().GetDuration());
                        durationController.FrostLanceTargetEffect(slownessPercentage);
                    }
                    
                    if (currentFrostLanceStack == maxFrostLanceStack)
                    {
                        CharacterBaseController baseController = GetComponent<CharacterBaseController>();

                        GameObject GO = effectsList[i];
                        Destroy(GO);
                        effectsList.Remove(GO);
                        baseController.SetMovementSpeed(baseController.GetCharacterStats().GetBaseMovementSpeed());
                        slownessPercentage = 0;
                        currentFrostLanceStack = 0;
                    }
                }
                return;
            }
        }
        ApplyStatusEffect();
    }

    private void ApplyStatusEffect()
    {
        GameObject GO = Instantiate(statusEffectPrefab, charactersEffectsPanel.transform);
        GO.GetComponent<EffectDurationController>().SetEffectDurationInfo(usedSpellInfo.GetStatusEffect(), this, hitOrCrit);
        effectsList.Add(GO);
    }

    public void StatusEffectInfoGathering(AbilitySO usedSpellInfo, string hitOrCrit)
    {
        this.usedSpellInfo = usedSpellInfo;
        this.hitOrCrit = hitOrCrit;

        CheckActiveEffects();
    }

    public void DisableEffectsList()
    {
        for (int i = 0; i < effectsList.Count; i++)
        {
            foreach (Transform child in effectsList[i].transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void EnableEffectsList()
    {
        for (int i = 0; i < effectsList.Count; i++)
        {
            foreach (Transform child in effectsList[i].transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public void RemoveGOFromList(GameObject objectToRemove)
    {
        effectsList.Remove(objectToRemove);
    }

    public void RemoveAllEffects()
    {
        for (int i = 0; i < effectsList.Count; i++)
        {
            GameObject GO = effectsList[i];
            Destroy(GO);
            effectsList.Remove(GO);
        }
    }
}
