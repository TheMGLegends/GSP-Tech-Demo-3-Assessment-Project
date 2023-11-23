using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesController : MonoBehaviour
{
    public List<AbilitySO.AbilityTypes> abilityTypesList = new();
    [SerializeField] private List<AbilitySO> abilityStatsList = new();
    [SerializeField] private Dictionary<AbilitySO.AbilityTypes, AbilitySO> abilityDictionary = new();

    private AbilitySO.AbilityTypes currentAbility;

    [SerializeField] private GameObject castingBar;

    private Slider castingBarSlider;
    private TMP_Text abilityNameText;

    private float castingInterval;
    private float currentActivationTime;

    private bool isCasting;

    public bool GetIsCasting() => isCasting;

    private void Start()
    {
        for (int i = 0; i < abilityTypesList.Count; i++)
        {
            abilityDictionary.Add(abilityTypesList[i], abilityStatsList[i]);
        }

        if (castingBar != null)
        {
            castingBarSlider = castingBar.GetComponent<Slider>();
            abilityNameText = castingBar.GetComponentInChildren<TMP_Text>();
        }

        ActivateCastingUI(false);
    }

    private void Update()
    {
        if (isCasting)
        {
            currentActivationTime += Time.deltaTime;

            castingBarSlider.value = currentActivationTime;

            if (currentActivationTime > castingInterval)
            {
                currentActivationTime = 0;

                GameObject GO = Instantiate(ReferenceManager.Instance.spellPrefab, ReferenceManager.Instance.playerObject.transform.position, Quaternion.identity);
                GO.GetComponent<SpriteRenderer>().sprite = abilityDictionary[currentAbility].GetAbilitySprite();

                castingBarSlider.value = castingBarSlider.minValue;
                abilityNameText.text = "";

                isCasting = false;
            }
        }
    }

    public void ActivateCastingUI(bool activate)
    {
        if (activate)
        {
            castingBarSlider.value = castingBarSlider.minValue;
            abilityNameText.text = "";

            castingBar.SetActive(true);
            gameObject.SetActive(true);
        }
        else
        {
            castingBar.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    public void AbilityPress(int abilityType)
    {
        switch ((AbilitySO.AbilityTypes)abilityType)
        {
            case AbilitySO.AbilityTypes.ArcaneMissile:
                currentAbility = AbilitySO.AbilityTypes.ArcaneMissile;
                break;
            case AbilitySO.AbilityTypes.Fireball:
                currentAbility = AbilitySO.AbilityTypes.Fireball;
                break;
            case AbilitySO.AbilityTypes.FrostLance:
                currentAbility = AbilitySO.AbilityTypes.FrostLance;
                break;
            case AbilitySO.AbilityTypes.MageArmor:
                currentAbility = AbilitySO.AbilityTypes.MageArmor;
                break;
            default:
                break;
        }

        if (abilityDictionary.ContainsKey(currentAbility))
        {
            isCasting = true;
            currentActivationTime = 0;
            castingInterval = abilityDictionary[currentAbility].GetCastingTime();
            castingBarSlider.maxValue = abilityDictionary[currentAbility].GetCastingTime();
            abilityNameText.text = abilityDictionary[currentAbility].GetAbilityName();
        }
    }
}
