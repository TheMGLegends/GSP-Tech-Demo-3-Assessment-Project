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
    private float currentCastingTime;
    private float tickInterval;

    private float manaCost;

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
        if (isCasting && (currentAbility == AbilitySO.AbilityTypes.ArcaneMissile || currentAbility == AbilitySO.AbilityTypes.Fireball))
        {
            castingParticles.SetActive(true);

            currentCastingTime += Time.deltaTime;
            castingBarSlider.value = currentCastingTime / castingInterval;

            if (currentCastingTime > castingInterval)
            {
                currentCastingTime = 0;
                isCasting = false;
                castingParticles.SetActive(false);

                InstantiateSpell();
                DeductPlayersMana();
            }

            if (currentAbility == AbilitySO.AbilityTypes.ArcaneMissile)
            {
                if (ReferenceManager.Instance.playerObject.GetComponent<PlayerController>().GetMovementInput() != Vector2.zero)
                {
                    currentCastingTime = 0;
                    tickInterval = castingInterval / abilityDictionary[currentAbility].GetNumberOfCasts();
                    castingBarSlider.value = castingBarSlider.minValue;
                    StartCoroutine(ParticleCoroutine(ReferenceManager.Instance.playerObject.GetComponent<PlayerAnimationController>().GetAnimator().GetCurrentAnimatorClipInfo(0).Length));
                }

                if (currentCastingTime > tickInterval)
                {
                    tickInterval++;

                    InstantiateSpell();
                    DeductPlayersMana();
                }
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
        if (!abilityDictionary.ContainsKey(ability.GetAbilityType()))
            return;

        currentAbility = ability.GetAbilityType();

        isCasting = true;
        currentCastingTime = 0;
        castingInterval = abilityDictionary[currentAbility].GetCastingTime();
        abilityNameText.text = abilityDictionary[currentAbility].GetAbilityName();
        manaCost = abilityDictionary[currentAbility].GetManaCost();

        if (ability.GetCastingTime() <= 0)
        {
            castingBarSlider.value = castingBarSlider.maxValue;
        }
        else
        {
            castingBarSlider.value = castingBarSlider.minValue;
        }

        switch (currentAbility)
        {
            case AbilitySO.AbilityTypes.ArcaneMissile:
                ArcaneMissileAbility();
                break;
            case AbilitySO.AbilityTypes.Fireball:
                FireballAbility();
                break;
            case AbilitySO.AbilityTypes.FrostLance:
                FrostLanceAbility();
                break;
            case AbilitySO.AbilityTypes.MageArmor:
                MageArmorAbility();
                break;
        }
    }

    // Casting Spell:
    private void ArcaneMissileAbility()
    {
        tickInterval = castingInterval / abilityDictionary[currentAbility].GetNumberOfCasts();
        manaCost /= abilityDictionary[currentAbility].GetNumberOfCasts();
    }

    // Casting Spell:
    private void FireballAbility()
    {

    }

    // Instant Spell:
    private void FrostLanceAbility()
    {
        castingParticles.SetActive(true);
        StartCoroutine(ParticleCoroutine(ReferenceManager.Instance.playerObject.GetComponent<PlayerAnimationController>().GetAnimator().GetCurrentAnimatorClipInfo(0).Length));
        InstantiateSpell();
        DeductPlayersMana();
    }

    // Instant Spell:
    private void MageArmorAbility()
    {
        castingParticles.SetActive(true);
        StartCoroutine(ParticleCoroutine(ReferenceManager.Instance.playerObject.GetComponent<PlayerAnimationController>().GetAnimator().GetCurrentAnimatorClipInfo(0).Length));
        DeductPlayersMana();
    }

    private void InstantiateSpell()
    {
        GameObject GO = Instantiate(ReferenceManager.Instance.spellPrefab, ReferenceManager.Instance.playerObject.transform.position, Quaternion.identity);
        GO.GetComponent<SpellController>().GetTargetAndAbilityInfo(ReferenceManager.Instance.playerObject.GetComponent<CharacterBaseController>().GetTarget(), abilityDictionary[currentAbility]);
        GO.GetComponent<SpriteRenderer>().sprite = abilityDictionary[currentAbility].GetAbilitySprite();
    }

    private IEnumerator ParticleCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        isCasting = false;
        castingParticles.SetActive(false);
    }

    private void DeductPlayersMana()
    {
        ReferenceManager.Instance.playerObject.GetComponent<CharacterBaseController>().ReduceMana(manaCost);
        ReferenceManager.Instance.playerObject.GetComponent<CharacterBaseController>().GetCharacterHUDController().SetMana(ReferenceManager.Instance.playerObject.GetComponent<PlayerController>().GetMana());
    }
}
