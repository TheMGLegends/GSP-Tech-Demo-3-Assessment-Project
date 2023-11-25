using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class StatusEffectController : MonoBehaviour
{
    [SerializeField] private GameObject charactersEffectsPanel;
    private List<GameObject> effectsList = new();
    private GameObject statusEffectPrefab;

    private AbilitySO usedSpellInfo;

    private readonly int maxFrostLanceStack = 3;

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
            if (usedSpellInfo.GetStatusEffect() == effectsList[i].GetComponent<EffectDurationController>().GetStatusEffect())
            {
                if (effectsList[i].GetComponent<EffectDurationController>().GetStatusEffect().GetStatusEffectType() == StatusEffectSO.StatusEffectTypes.FrostLanceEffect)
                {
                    if (effectsList[i].GetComponent<EffectDurationController>().GetCurrentFrostLanceStack() < maxFrostLanceStack)
                        effectsList[i].GetComponent<EffectDurationController>().IncrementFrostLanceStack();
                    else if (effectsList[i].GetComponent<EffectDurationController>().GetCurrentFrostLanceStack() >= maxFrostLanceStack)
                    {
                        GameObject GO = effectsList[i];
                        Destroy(GO);
                        effectsList.Remove(GO);
                        gameObject.GetComponent<CharacterBaseController>().SetMovementSpeed(gameObject.GetComponent<CharacterBaseController>().GetCharacterStats().GetBaseMovementSpeed());
                    } 
                    else if (effectsList[i].GetComponent<EffectDurationController>().GetCurrentFrostLanceStack() == 0)
                        continue;
                }
                return;
            }
        }

        ApplyStatusEffect();
    }

    private void ApplyStatusEffect()
    {
        GameObject GO = Instantiate(statusEffectPrefab, charactersEffectsPanel.transform);
        GO.GetComponent<EffectDurationController>().SetEffectDurationInfo(usedSpellInfo.GetStatusEffect(), this);
        effectsList.Add(GO);
    }

    public void StatusEffectInfoGathering(AbilitySO usedSpellInfo)
    {
        this.usedSpellInfo = usedSpellInfo;

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

    public int SearchEffectsList(StatusEffectSO.StatusEffectTypes effectType)
    {
        for (int i = 0; i < effectsList.Count; i++)
        {
            if (effectsList[i].GetComponent<EffectDurationController>().GetStatusEffect().GetStatusEffectType() == effectType)
            {
                return effectsList[i].GetComponent<EffectDurationController>().GetCurrentFrostLanceStack();
            }
        }
        return 0;
    }
}
