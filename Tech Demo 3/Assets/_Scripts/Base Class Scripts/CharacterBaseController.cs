using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterBaseController : MonoBehaviour
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

    protected Vector2 startingPosition;
    protected bool isDead;

    protected CharacterAnimationController characterAnimationController;
    protected BoxCollider2D characterCollider;

    public float GetHealth() => health;
    public float GetDefenseMultiplier() => defenseMultiplier;
    public GameObject GetDamagePopupObject() => damagePopupObject;
    public Animator GetDamagePopupAnimator() => damagePopupAnimator;
    public TextMesh GetDamagePopupText() => damagePopupText;
    public bool GetIsDead() => isDead;
    public CharacterAnimationController GetCharacterAnimationController() => characterAnimationController;

    private void Awake()
    {
        InitializeStats();
    }

    protected virtual void Start()
    {
        damagePopupObject = Instantiate(ReferenceManager.Instance.damagePopupPrefab,
                                  new Vector2(transform.position.x, transform.position.y + damagePopupYOffset),
                                  Quaternion.identity, transform);
        damagePopupObject.SetActive(false);

        damagePopupText = damagePopupObject.transform.GetChild(0).GetComponent<TextMesh>();
        damagePopupAnimator = damagePopupObject.transform.GetChild(0).GetComponent<Animator>();

        startingPosition = transform.position;

        characterAnimationController = GetComponent<CharacterAnimationController>();
        characterCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void InitializeStats()
    {
        health = characterStats.GetBaseHealth();
        movementSpeed = characterStats.GetBaseMovementSpeed();
        defenseMultiplier = characterStats.GetBaseDefenseMultiplier();
        meleeDamageAmount = characterStats.GetNormalDamage();
        normalAttackInterval = characterStats.GetNormalAttackSpeed();
    }

    public void ReduceHealth(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            isDead = true;
            DeathAction();
        }
    }

    protected virtual void DeathAction()
    {
        characterCollider.enabled = false;
    }

    protected virtual void AfterDeath()
    {
        isDead = false;
    }
}
