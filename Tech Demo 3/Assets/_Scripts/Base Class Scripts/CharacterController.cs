using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] protected CharacterStatsSO characterStats;

    protected float health;
    protected float movementSpeed;
    protected float defenseMultiplier;
    protected float meleeDamageAmount;
    protected float normalAttackInterval;

    protected GameObject damagePopupObject;
    protected Animator damagePopupAnimator;
    protected TextMesh damagePopupText;
    protected float damagePopupYOffset = 1.5f;

    public float GetHealth() => health;

    private void Awake()
    {
        InitializeStats();
    }

    protected virtual void Start()
    {
        damagePopupObject = Instantiate(ReferenceManager.Instance.damagePopupPrefab,
                                  new Vector2(transform.position.x, transform.position.y + damagePopupYOffset),
                                  Quaternion.identity);
        damagePopupObject.SetActive(false);

        damagePopupText = damagePopupObject.transform.GetChild(0).GetComponent<TextMesh>();
        damagePopupAnimator = damagePopupObject.transform.GetChild(0).GetComponent<Animator>();
    }

    protected virtual void InitializeStats()
    {
        health = characterStats.GetBaseHealth();
        movementSpeed = characterStats.GetBaseMovementSpeed();
        defenseMultiplier = characterStats.GetBaseDefenseMultiplier();
        meleeDamageAmount = characterStats.GetNormalDamage();
        normalAttackInterval = characterStats.GetNormalAttackSpeed();
    }
}
