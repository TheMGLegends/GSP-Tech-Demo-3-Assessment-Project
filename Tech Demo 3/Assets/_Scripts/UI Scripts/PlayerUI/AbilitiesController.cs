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
    private GameObject castingParticles;

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
        castingParticles = ReferenceManager.Instance.playerObject.transform.GetChild(1).gameObject;
    }

    private void Update()
    {
        if (isCasting)
        {
            castingParticles.SetActive(true);

            currentActivationTime += Time.deltaTime;

            if (abilityDictionary[currentAbility].GetCastingTime() > 0)
            {
                castingBarSlider.value = currentActivationTime;
            }

            if (currentActivationTime > castingInterval)
            {
                currentActivationTime = 0;

                GameObject GO = Instantiate(ReferenceManager.Instance.spellPrefab, ReferenceManager.Instance.playerObject.transform.position, Quaternion.identity);
                GO.GetComponent<SpriteRenderer>().sprite = abilityDictionary[currentAbility].GetAbilitySprite();

                isCasting = false;
                castingParticles.SetActive(false);
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

    public void AbilityPress(AbilitySO ability)
    {
        //switch (ability.GetAbilityType())
        //{
        //    case AbilitySO.AbilityTypes.ArcaneMissile:
        //        break;
        //    case AbilitySO.AbilityTypes.Fireball:
        //        break;
        //    case AbilitySO.AbilityTypes.FrostLance:
        //        break;
        //    case AbilitySO.AbilityTypes.MageArmor:
        //        break;
        //    default:
        //        break;
        //}

        castingBarSlider.value = castingBarSlider.minValue;
        abilityNameText.text = "";

        if (abilityDictionary.ContainsKey(ability.GetAbilityType()))
        {
            isCasting = true;
            currentAbility = ability.GetAbilityType();
            currentActivationTime = 0;
            castingInterval = abilityDictionary[currentAbility].GetCastingTime();

            if (castingInterval <= 0)
            {
                castingBarSlider.maxValue = 1.0f;
                castingBarSlider.value = castingBarSlider.maxValue;
            }
            else
            {
                castingBarSlider.maxValue = abilityDictionary[currentAbility].GetCastingTime();
            }

            abilityNameText.text = abilityDictionary[currentAbility].GetAbilityName();
        }
    }
}
