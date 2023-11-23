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
    protected float normalDamageAmount;
    protected float normalAttackInterval;

    protected bool canAttack;

    protected Vector3 startingPosition;
    protected bool isDead;

    protected CharacterAnimationController characterAnimationController;

    protected GameObject target;

    public float GetHealth() => health;
    public float GetDefenseMultiplier() => defenseMultiplier;
    public bool GetIsDead() => isDead;
    public CharacterAnimationController GetCharacterAnimationController() => characterAnimationController;
    public GameObject GetTarget() => target;
    public Vector2 GetStartingPosition() => startingPosition;
    public CharacterStatsSO GetCharacterStats() => characterStats;
    public void SetTarget(GameObject target) { this.target = target; }
    public void SetCanAttack(bool canAttack) { this.canAttack = canAttack; }

    private void Awake()
    {
        InitializeStats();
    }

    protected virtual void Start()
    {
        startingPosition = transform.position;
        characterAnimationController = GetComponent<CharacterAnimationController>();
    }

    protected virtual void InitializeStats()
    {
        health = characterStats.GetBaseHealth();
        movementSpeed = characterStats.GetBaseMovementSpeed();
        defenseMultiplier = characterStats.GetBaseDefenseMultiplier();
        normalDamageAmount = characterStats.GetNormalDamage();
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
    }

    protected virtual void AfterDeath()
    {
        isDead = false;
    }
}
