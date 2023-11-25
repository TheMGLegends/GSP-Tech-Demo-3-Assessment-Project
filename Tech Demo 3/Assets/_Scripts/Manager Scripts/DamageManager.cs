using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance { get; private set; }

    [SerializeField] private GameObject damagePopupPrefab;

    [SerializeField] private float damagePopupMaxYOffset = 1.5f;
    [SerializeField] private float damagePopupMinYOffset = 1.0f;

    [SerializeField] private float damagePopupMaxXOffset = 0.5f;
    [SerializeField] private float damagePopupMinXOffset = -0.5f;

    private const int hitChance = 80;
    private const int critChance = 20;

    private string returnValue;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public string Damage(float damage, CharacterBaseController targetController, CharacterHUDController targetHUDController, Color textColor)
    {
        if (Random.Range(0, 101) <= hitChance)
        {
            damage *= Random.Range(0.75f, 1.25f) * targetController.GetDefenseMultiplier();

            if (Random.Range(0, 101) <= critChance)
            {
                damage *= 2;
                returnValue = AttackResultStrings.hasCrit;
            }
            else
                returnValue = AttackResultStrings.hasHit;

            GameObject GO = Instantiate(damagePopupPrefab, 
                new Vector2(targetController.transform.position.x + Random.Range(damagePopupMinXOffset, damagePopupMaxXOffset), 
                            targetController.transform.position.y + Random.Range(damagePopupMinYOffset, damagePopupMaxYOffset)), 
                Quaternion.identity);
            
            GO.GetComponentInChildren<TextMesh>().color = textColor;
            GO.GetComponentInChildren<TextMesh>().text = ((int)damage).ToString();

            targetController.ReduceHealth((int)damage);
            targetHUDController.SetHealth(targetController.GetHealth());

            Destroy(GO, 1);
        }
        else
        {
            returnValue = AttackResultStrings.hasMissed;
        }

        return returnValue;
    }
}
