using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    private AbilitySO abilityInfo;
    private GameObject target;

    private float spellSpeed = 5f;

    private Color floatingNumberColor;

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
        Debug.Log(collision.gameObject.tag);

        if (collision.gameObject.CompareTag(target.tag))
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
                DamageManager.Instance.Damage(abilityInfo.GetBasePower(), target.GetComponent<CharacterBaseController>(), target.GetComponent<CharacterBaseController>().GetCharacterHUDController(), floatingNumberColor);

            Destroy(gameObject);
        }
    }


    public void GetTargetAndAbilityInfo(GameObject target, AbilitySO abilityInfo)
    {
        this.target = target;
        this.abilityInfo = abilityInfo;
    }
}
