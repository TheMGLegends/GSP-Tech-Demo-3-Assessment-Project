using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesController : MonoBehaviour
{
    public List<AbilitySO.AbilityTypes> abilityTypesList = new();
    [SerializeField] private List<AbilitySO> abilityStatsList = new();
    [SerializeField] private Dictionary<AbilitySO.AbilityTypes, AbilitySO> abilityTypesDictionary = new();
    
    [SerializeField] private List<GameObject> abilityButtonsList = new();
    [SerializeField] private Dictionary<AbilitySO.AbilityTypes, GameObject> abilityButtonsDictionary = new();

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
            abilityTypesDictionary.Add(abilityTypesList[i], abilityStatsList[i]);
            abilityButtonsDictionary.Add(abilityTypesList[i], abilityButtonsList[i]);
        }

        if (castingBar != null)
        {
            castingBarSlider = castingBar.GetComponent<Slider>();
            abilityNameText = castingBar.GetComponentInChildren<TMP_Text>();
        }

        castingParticles = ReferenceManager.Instance.playerObject.transform.GetChild(1).gameObject;

        CheckSpellUsability();

        ActivateCastingUI(false);
    }

    private void Update()
    {
        CastSpells();
    }

    public void CheckSpellUsability()
    {
        for (int i = 0; i < abilityTypesDictionary.Count; i++)
        {
            GameObject buttonChild = abilityButtonsDictionary[(AbilitySO.AbilityTypes)i].transform.GetChild(2).gameObject;
            GameObject unusableChild = abilityButtonsDictionary[(AbilitySO.AbilityTypes)i].transform.GetChild(4).gameObject;

            if (abilityTypesDictionary[(AbilitySO.AbilityTypes)i].GetManaCost() > ReferenceManager.Instance.playerObject.GetComponent<PlayerController>().GetMana())
            {
                buttonChild.GetComponent<Button>().interactable = false;
                unusableChild.SetActive(true);
            }
            else
            {
                unusableChild.SetActive(false);
                buttonChild.GetComponent<Button>().interactable = true;
            }
        }
    }

    private void CastSpells()
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
                    isCasting = false;
                    abilityNameText.text = "";
                    currentCastingTime = 0;
                    tickInterval = castingInterval / abilityTypesDictionary[currentAbility].GetNumberOfCasts();
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
        isCasting = false;
        castingParticles.SetActive(false);

        castingBarSlider.value = castingBarSlider.minValue;
        abilityNameText.text = "";

        if (activate)
        {
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
        if (!abilityTypesDictionary.ContainsKey(ability.GetAbilityType()))
            return;

        currentAbility = ability.GetAbilityType();

        currentCastingTime = 0;
        castingInterval = abilityTypesDictionary[currentAbility].GetCastingTime();
        abilityNameText.text = abilityTypesDictionary[currentAbility].GetAbilityName();
        manaCost = abilityTypesDictionary[currentAbility].GetManaCost();

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
        if (ReferenceManager.Instance.playerObject.GetComponent<PlayerController>().GetMovementInput() == Vector2.zero)
            isCasting = true;

        tickInterval = castingInterval / abilityTypesDictionary[currentAbility].GetNumberOfCasts();
        manaCost /= abilityTypesDictionary[currentAbility].GetNumberOfCasts();
    }

    // Casting Spell:
    private void FireballAbility()
    {
        isCasting = true;
    }

    // Instant Spell:
    private void FrostLanceAbility()
    {
        isCasting = true;
        castingParticles.SetActive(true);
        StartCoroutine(ParticleCoroutine(ReferenceManager.Instance.playerObject.GetComponent<PlayerAnimationController>().GetAnimator().GetCurrentAnimatorClipInfo(0).Length));
        InstantiateSpell();
        DeductPlayersMana();
    }

    // Instant Spell:
    private void MageArmorAbility()
    {
        isCasting = true;
        castingParticles.SetActive(true);
        StartCoroutine(ParticleCoroutine(ReferenceManager.Instance.playerObject.GetComponent<PlayerAnimationController>().GetAnimator().GetCurrentAnimatorClipInfo(0).Length));
        DeductPlayersMana();
    }

    private void InstantiateSpell()
    {
        GameObject GO = Instantiate(ReferenceManager.Instance.spellPrefab, ReferenceManager.Instance.playerObject.transform.position, Quaternion.identity);
        GO.GetComponent<SpellController>().GetTargetAndAbilityInfo(ReferenceManager.Instance.playerObject.GetComponent<CharacterBaseController>().GetTarget(), abilityTypesDictionary[currentAbility]);
        GO.GetComponent<SpriteRenderer>().sprite = abilityTypesDictionary[currentAbility].GetAbilitySprite();
    }

    private IEnumerator ParticleCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentAbility == AbilitySO.AbilityTypes.MageArmor || currentAbility == AbilitySO.AbilityTypes.FrostLance)
            isCasting = false;

        castingParticles.SetActive(false);
    }

    private void DeductPlayersMana()
    {
        ReferenceManager.Instance.playerObject.GetComponent<CharacterBaseController>().ReduceMana(manaCost);
        ReferenceManager.Instance.playerObject.GetComponent<CharacterBaseController>().GetCharacterHUDController().SetMana(ReferenceManager.Instance.playerObject.GetComponent<PlayerController>().GetMana());
        
        CheckSpellUsability();
        SpellCooldown();
    }

    private void SpellCooldown()
    {
        GameObject buttonChild = abilityButtonsDictionary[currentAbility].transform.GetChild(2).gameObject;
        GameObject cooldownChild = abilityButtonsDictionary[currentAbility].transform.GetChild(3).gameObject;
        GameObject unusableChild = abilityButtonsDictionary[currentAbility].transform.GetChild(4).gameObject;

        if (abilityTypesDictionary[currentAbility].GetCooldown() > 0 && !unusableChild.activeSelf)
        {
            buttonChild.GetComponent<Button>().interactable = false;
            cooldownChild.SetActive(true);
            cooldownChild.GetComponent<CooldownController>().ActivateCooldown(abilityTypesDictionary[currentAbility].GetCooldown());
        }
    }
}
