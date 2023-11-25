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

    private int currentFrostLanceStack;
    private readonly int maxFrostLanceStack = 5;

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
        GO.GetComponent<EffectDurationController>().SetStatusEffect(usedSpellInfo.GetStatusEffect());
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
            effectsList[i].SetActive(false);
        }
    }

    public void EnableEffectsList()
    {
        for (int i = 0; i < effectsList.Count; i++)
        {
            effectsList[i].SetActive(true);
        }
    }
}
