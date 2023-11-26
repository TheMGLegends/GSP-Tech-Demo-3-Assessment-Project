using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    private GameObject caster;
    private GameObject target;
    private AbilitySO abilityInfo;

    private readonly float spellSpeed = 5f;

    private Color floatingNumberColor;

    private string spellResult;

    private void Update()
    {
        if (target.GetComponent<CharacterBaseController>().GetIsDead() && transform.position == target.transform.position)
            Destroy(gameObject);

        Vector3 look = transform.InverseTransformPoint(target.transform.position);
        float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg;

        transform.Rotate(0, 0, angle);

        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, spellSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == target)
        {
            if (target.CompareTag("Enemy"))
            {
                floatingNumberColor = Color.red;
            }
            else if (target.CompareTag("Player"))
            {
                floatingNumberColor = Color.yellow;
            }

            if (!target.GetComponent<CharacterBaseController>().GetIsDead())
            {
                if (target.GetComponent<StatusEffectController>().GetCurrentFrostLanceStack() > 3 && abilityInfo.GetAbilityType() == AbilitySO.AbilityTypes.FrostLance)
                    DamageManager.Instance.Damage(abilityInfo.GetBasePower() + 20, target.GetComponent<CharacterBaseController>(), target.GetComponent<CharacterBaseController>().GetCharacterHUDController(), floatingNumberColor);
                else
                    spellResult = DamageManager.Instance.Damage(abilityInfo.GetBasePower(), target.GetComponent<CharacterBaseController>(), target.GetComponent<CharacterBaseController>().GetCharacterHUDController(), floatingNumberColor);

                if (spellResult != AttackResultStrings.hasMissed)
                    CheckAffectedEntity();
            }

            Destroy(gameObject);
        }
    }

    private void CheckAffectedEntity()
    {
        switch (abilityInfo.GetAbilityType())
        {
            case AbilitySO.AbilityTypes.ArcaneMissile:
                if (spellResult == AttackResultStrings.hasCrit)
                    caster.GetComponent<StatusEffectController>().StatusEffectInfoGathering(abilityInfo, spellResult);
                break;
            case AbilitySO.AbilityTypes.Fireball:
                    target.GetComponent<StatusEffectController>().StatusEffectInfoGathering(abilityInfo, spellResult);
                break;
            case AbilitySO.AbilityTypes.FrostLance:
                    target.GetComponent<StatusEffectController>().StatusEffectInfoGathering(abilityInfo, spellResult);
                break;
            case AbilitySO.AbilityTypes.ToxicSpit:
                    target.GetComponent<StatusEffectController>().StatusEffectInfoGathering(abilityInfo, spellResult);
                break;
        }
    }

    public void GatherInfo(GameObject caster, GameObject target, AbilitySO abilityInfo)
    {
        this.caster = caster;
        this.target = target;
        this.abilityInfo = abilityInfo;
    }
}
